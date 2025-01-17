using Abp.Dependency;
using Abp.Json;
using Abp.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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

        public void NotifyToWebhookUrl(string webhookMessage, string url, string name = "")
        {
            if (_isNotifyToMezonWebhook != "true")
            {
                logger.LogInformation("_isNotifyToMezonWebhook=" + _isNotifyToMezonWebhook + " => stop");
                return;
            }

            var blockMarkdowns = GetMarkdownsForBlock(webhookMessage);
            //var links = GetLinksFromMessage(webhookMessage);

            var content = new SendingContent
            {
                type = "APP",
                message = new Message
                {
                    t = webhookMessage,
                    mk = blockMarkdowns,
                }
            };
            Post(url, content);
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

        private List<Markdown> GetMarkdownsForBlock(string message)
        {
            List<Markdown> markdowns = new List<Markdown>();
            string pattern = @"```(.*?)```";

            MatchCollection matches = Regex.Matches(message, pattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                int start = match.Index;
                int end = match.Index + match.Length;
                markdowns.Add(new Markdown { type = "t", s = start, e = end });
            }
            return markdowns;
        }
        private List<Link> GetLinksFromMessage(string message)
        {
            List<Link> links = new List<Link>();
            string pattern = @"https?://[^\s]+";
            int start = 0;
            int end = 0;

            MatchCollection matches = Regex.Matches(message, pattern);

            foreach (Match match in matches)
            {
                start = match.Index;
                end = start + match.Length;
                links.Add(new Link { s = start, e = end });
            }
            return links;
        }
    }
}
