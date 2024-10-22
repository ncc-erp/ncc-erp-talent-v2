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
using TalentV2.Entities;
using TalentV2.NccCore;

namespace TalentV2.WebServices.ExternalServices.MezonWebhooks
{
    public class MezonWebhookService : BaseWebService
    {
        protected IWorkScope WorkScope;
        private const string serviceName = "MezonWebhookService";
        private readonly string _isNotifyToMezonWebhook;

        public MezonWebhookService(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ILogger<MezonWebhookService> logger, 
            IAbpSession abpSession)
           : base(httpClient, logger, abpSession)
        {
            _isNotifyToMezonWebhook = configuration.GetValue<string>($"{serviceName}:EnableMezonWebhookNotification");
            WorkScope = IocManager.Instance.Resolve<IWorkScope>();
        }

        public async Task NotifyToWebhookUrl(string webhookMessage, string url)
        {
            if (_isNotifyToMezonWebhook != "true")
            {
                logger.LogInformation("_isNotifyToMezonWebhook=" + _isNotifyToMezonWebhook + " => stop");
                return;
            }

            List<Task> taskList = new List<Task>();
            var webhooks = WorkScope.GetAll<MezonWebhook>().Where(m => m.Url == url && m.IsActive).ToList();

            if (webhooks.Count == 0)
            {
                logger.LogInformation($"There is no available Mezon Webhook with Url: {url}!");
                return;
            }

            webhooks.ForEach(w =>
            {
                var content = new SendingContent
                {
                    type = "APP",
                    message = new Message
                    {
                        t = webhookMessage,
                        username = w.Name
                    }
                };
                Task task = Task.Run(() => Post(w.Url, content));
                taskList.Add(task);
            });
            await Task.WhenAll(taskList);
        }

        public async Task NotifyToWebhookWithUsername(string webhookMessage, string name)
        {
            if (_isNotifyToMezonWebhook != "true")
            {
                logger.LogInformation("_isNotifyToMezonWebhook=" + _isNotifyToMezonWebhook + " => stop");
                return;
            }

            List<Task> taskList = new List<Task>();
            var webhooks = WorkScope.GetAll<MezonWebhook>().Where(m => m.Name == name && m.IsActive).ToList();

            if (webhooks.Count == 0)
            {
                logger.LogInformation($"There is no available Mezon Webhook with username: {name}!");
                return;
            }

            webhooks.ForEach(w =>
            {
                var content = new SendingContent
                {
                    type = "APP",
                    message = new Message
                    {
                        t = webhookMessage,
                        username = w.Name
                    }
                };
                Task task = Task.Run(() => Post(w.Url, content));
                taskList.Add(task);
            });
            await Task.WhenAll(taskList);
        }

        public async Task NotifyToWebhookId(string webhookMessage, long id)
        {
            if (_isNotifyToMezonWebhook != "true")
            {
                logger.LogInformation("_isNotifyToMezonWebhook=" + _isNotifyToMezonWebhook + " => stop");
                return;
            }

            try
            {
                MezonWebhook webhook = await WorkScope.GetAsync<MezonWebhook>(id);
                if (webhook == null)
                {
                    logger.LogInformation("Mezon Webhook Id have not already existed!");
                    return;
                }
                if (!webhook.IsActive)
                {
                    logger.LogInformation($"Mezon Webhook Id: {id} is not actice!");
                    return;
                }
                var content = new SendingContent
                {
                    type = "APP",
                    message = new Message
                    {
                        t = webhookMessage,
                        username = webhook.Name
                    }
                };
                Post(webhook.Url, content);
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex.Message);
            }    
        }

        public async Task NotifyToWebhookIds(string webhookMessage, params int[] ids)
        {
            if (_isNotifyToMezonWebhook != "true")
            {
                logger.LogInformation("_isNotifyToMezonWebhook=" + _isNotifyToMezonWebhook + " => stop");
                return;
            }

            List<Task> taskList = new List<Task>();
            List<MezonWebhook> webhooks = new List<MezonWebhook>();

            foreach (int id in ids)
            {
                try
                {
                    MezonWebhook webhook = await WorkScope.GetAsync<MezonWebhook>(id);
                    if (webhook == null)
                        logger.LogInformation($"Mezon Webhook Id: {id} have not already existed!");
                    else if (!webhook.IsActive)
                        logger.LogInformation($"Mezon Webhook Id: {id} is not actice!");
                    else
                        webhooks.Add(webhook);
                }
                catch (Exception ex) {
                    logger.LogInformation(ex.Message);
                }
            }

            webhooks.ForEach(w =>
            {
                var content = new SendingContent
                {
                    type = "APP",
                    message = new Message
                    {
                        t = webhookMessage,
                        username = w.Name
                    }
                };
                Task task = Task.Run(() => Post(w.Url, content));
                taskList.Add(task);
            });
            await Task.WhenAll(taskList);
        }

        protected override void Post(string url, object input)
        {
            string strInput = JsonConvert.SerializeObject(input);
            try
            {
                logger.LogInformation($"Post: {url} input: {strInput}");
                var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");
                HttpClient.PostAsync(url, contentString);
            }
            catch (Exception e)
            {
                logger.LogError($"Post: {url} input: {strInput} Error: {e.Message}");
            }
        }
    }
}
