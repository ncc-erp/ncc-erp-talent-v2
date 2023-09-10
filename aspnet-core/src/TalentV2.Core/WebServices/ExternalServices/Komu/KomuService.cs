using Abp.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.Notifications.Komu;

namespace TalentV2.WebServices.ExternalServices.Komu
{
    public class KomuService : BaseWebService
    {
        private const string serviceName = "KomuService";
        private readonly string _channelIdDevMode;
        private readonly string _isNotifyToKomu;
        public KomuService(HttpClient httpClient, IConfiguration configuration, ILogger<KomuService> logger, IAbpSession abpSession) 
            : base(httpClient, logger, abpSession)
        {
            _channelIdDevMode = configuration.GetValue<string>($"{serviceName}:ChannelIdDevMode");
            _isNotifyToKomu = configuration.GetValue<string>($"{serviceName}:EnableKomuNotification");
        }

        public void NotifyToChannel(string komuMessage, string channelId)
        {
            if (_isNotifyToKomu != "true")
            {
                logger.LogInformation("_isNotifyToKomu=" + _isNotifyToKomu + " => stop");
                return;
            }
            var channelIdToSend = string.IsNullOrEmpty(_channelIdDevMode) ? channelId : _channelIdDevMode;
            Post(KomuUrlConstant.KOMU_CHANNELID, new { message = komuMessage, channelid = channelIdToSend });
        }
        public void SendMessageToUser(string username, string message)
        {
            Post(KomuUrlConstant.KOMU_USER_ONLY, new { username = username, message = message });
        }
    }
}
