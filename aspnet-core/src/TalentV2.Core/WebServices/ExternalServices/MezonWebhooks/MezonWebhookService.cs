using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Dictionary;
using TalentV2.Entities;
using TalentV2.NccCore;
using TalentV2.WebServices.ExternalServices.MezonWebhooks.Dtos;

namespace TalentV2.WebServices.ExternalServices.MezonWebhooks
{
    /// <summary>
    /// Service for sending webhook notifications to Mezon.
    /// It handles the logic of identifying relevant webhooks based on function names,
    /// checking if notifications are enabled, and sending messages asynchronously.
    /// </summary>
    public class MezonWebhookService : BaseWebService
    {
        protected IWorkScope WorkScope;
        protected readonly bool MezonWebhookNotificationEnabled = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MezonWebhookService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for making requests.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="logger">The logger for logging messages.</param>
        /// <param name="abpSession">The ABP session for accessing current user and tenant information.</param>
        public MezonWebhookService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<MezonWebhookService> logger,
            IAbpSession abpSession)
           : base(httpClient, logger, abpSession)
        {
            MezonWebhookNotificationEnabled = configuration.GetValue<bool>("MezonWebhookService:MezonWebhookNotificationEnabled");
            WorkScope = IocManager.Instance.Resolve<IWorkScope>();
        }

        /// <summary>
        /// Sends a message to Mezon webhooks that are configured for the specified function name.
        /// The message is sent asynchronously.
        /// </summary>
        /// <param name="webhookDto">The message data to send.</param>
        /// <param name="functionName">The name of the function triggering the webhook, used to filter relevant webhooks.</param>
        public void SendMessage(MezonWebhookMessageDto webhookDto, string functionName)
        {
            logger.LogInformation("Sending message to Mezon.");

            if (!MezonWebhookNotificationEnabled)
            {
                logger.LogInformation("Mezon notification feature has been disabled. MezonWebhookNotificationEnabled={value}.", MezonWebhookNotificationEnabled.ToString());
            }

            if (!DictionaryHelper.MessageFunctionDic.Keys.Any(x => x == functionName))
            {
                logger.LogInformation("Message function {functionName} is not supported.", functionName);
            }
            var webhooks = WorkScope.GetAll<MezonWebhook>().ToList();
            webhooks = webhooks.Where(x => x.Functions.Any(f => f == functionName)).ToList();

            Task.Run(async () =>
            {
                try
                {
                    List<Task> sendMessageTasks = new();
                    foreach (var webhook in webhooks)
                    {
                        if (!webhook.IsActive || webhook.IsDeleted)
                        {
                            logger.LogInformation("Mezon webhook {name} has been disabled.", webhook.Name);
                            continue;
                        }

                        Task task = Task.Run(() => Post(webhook.Url, webhookDto));
                        sendMessageTasks.Add(task);
                    }

                    await Task.WhenAll(sendMessageTasks);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "SendMessage {functionName} to mezon failed. Error: {error}", functionName, ex.Message);
                }
            });
        }

        /// <summary>
        /// Sends a message to a predefined list of Mezon webhooks.
        /// The message is sent asynchronously.
        /// </summary>
        /// <param name="webhookDto">The message data to send.</param>
        /// <param name="webhooks">A list of MezonWebhook entities to send the message to.</param>
        public void SendMessage(MezonWebhookMessageDto webhookDto, List<MezonWebhook> webhooks)
        {
            Task.Run(async () =>
            {
                try
                {
                    logger.LogInformation("Sending message to Mezon.");
                    if (!MezonWebhookNotificationEnabled)
                    {
                        logger.LogInformation("Mezon notification feature has been disabled. MezonWebhookNotificationEnabled={value}.", MezonWebhookNotificationEnabled.ToString());
                    }

                    List<Task> sendMessageTasks = new();
                    foreach (var webhook in webhooks)
                    {
                        if (!webhook.IsActive || webhook.IsDeleted)
                        {
                            logger.LogInformation("Mezon webhook {name} has been disabled.", webhook.Name);
                            continue;
                        }

                        Task task = Task.Run(() => Post(webhook.Url, webhookDto));
                        sendMessageTasks.Add(task);
                    }

                    await Task.WhenAll(sendMessageTasks);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "SendMessage to mezon failed. Error: {error}", ex.Message);
                }
            });
        }

        /// <summary>
        /// Sends a message to a single, specific Mezon webhook.
        /// The message is sent synchronously within this method's direct call to Post, but the Post method itself fires an asynchronous HTTP request.
        /// </summary>
        /// <param name="webhookDto">The message data to send.</param>
        /// <param name="webhook">The MezonWebhook entity to send the message to.</param>
        public void SendMessage(MezonWebhookMessageDto webhookDto, MezonWebhook webhook)
        {
            try
            {
                logger.LogInformation("Sending message to Mezon.");
                if (!MezonWebhookNotificationEnabled)
                {
                    logger.LogInformation("Mezon notification feature has been disabled. MezonWebhookNotificationEnabled={value}.", MezonWebhookNotificationEnabled.ToString());
                }

                if (!webhook.IsActive || webhook.IsDeleted)
                {
                    logger.LogInformation("Mezon webhook {name} has been disabled.", webhook.Name);
                }

                Post(webhook.Url, webhookDto);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SendMessage to mezon failed. Error: {error}", ex.Message);
            }
        }

        /// <summary>
        /// Posts the given input object as JSON to the specified URL.
        /// The actual HTTP POST request is performed asynchronously.
        /// JSON serialization ignores null values.
        /// </summary>
        /// <param name="url">The URL to post the data to.</param>
        /// <param name="input">The object to serialize as JSON and send.</param>
        protected override void Post(string url, object input)
        {
            string payload = JsonConvert.SerializeObject(input, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            try
            {
                logger.LogInformation("Post: {url}. Data: {payload}", url, payload);
                var contentString = new StringContent(payload, Encoding.UTF8, "application/json");
                Task.Run(async () => await HttpClient.PostAsync(url, contentString));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Post: {url}. Data: {payload}. Error: {error}", url, payload, ex.Message);
            }
        }
    }
}
