using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Threading;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.BackgroundWorker.CVTransformHelper;
using TalentV2.Configuration;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.Utils;
using TalentV2.WebServices.ExternalServices.Komu;
using Microsoft.Extensions.Configuration;

namespace TalentV2.BackgroundWorker
{
    public class CrawlCVFromFirebaseWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {

        private readonly ILogger<CrawlCVFromFirebaseWorker> _logger;
        private readonly CVAutomationFirebase _cVAutomationFirebase;
        protected readonly KomuService _komuService;
        protected readonly IConfiguration _configuration;
        public AutomationResult InternResult { get; private set; }
        public AutomationResult StaffResult { get; private set; }
        public CrawlCVFromFirebaseWorker(AbpTimer timer,
            ILogger<CrawlCVFromFirebaseWorker> logger,
            CVAutomationFirebase cVAutomationFirebase,
            KomuService komuService,
            IConfiguration configuration,
            ISettingManager settingManager) : base(timer)
        {
            _logger = logger;
            _komuService = komuService;
            _cVAutomationFirebase = cVAutomationFirebase;
            _configuration = configuration;
            Timer.RunOnStart = true;
            //Timer.Period = 5000;
            if (int.TryParse(settingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationRepeatTimeInMinutes), out var repeatTimeInMinutes))
            {
                Timer.Period = 1000 * 60 * repeatTimeInMinutes;
            }
            else
            {
                Timer.Period = 1000 * 60 * 60 * 24; // default repeat each 24 hour
            }

            InternResult = new AutomationResult();
            //StaffResult = new AutomationResult();
        }

        protected override void DoWork()
        {

            Logger.Info("CrawlCVFromFirebase start");
            if (!CheckTimeRule())
            {
                return;
            }
            try
            {
                AsyncHelper.RunSync(async () =>
                {
                    var internResult = await _cVAutomationFirebase.AutoCreateCVFromFirebase();
                    if (internResult != null)
                    {
                        InternResult.Success += internResult.Success;
                        //InternResult.Total += internResult.Total;
                    }
                    //var staffResult = await _cVAutomationFirebase.AutoCreateStaffCV();
                    //if (staffResult != null)
                    //{
                    //    StaffResult.Success += staffResult.Success;
                    //    StaffResult.Total += staffResult.Total;
                    //}
                });


                bool.TryParse(SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationEnabled), out bool enableNotify);
                //if (enableNotify && (InternResult.Success > 0 || StaffResult.Total > 0)) PreNotify();
                if (enableNotify && InternResult.Success > 0) PreNotify();


                _logger.LogInformation("Crawling data from Firebase completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while crawling data from Firebase.");
            }
        }
        private bool CheckTimeRule()
        {
            DateTime now = DateTimeUtils.GetNow();
            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
            {
                Logger.Info("Today is DayOff => stop");
                return false;
            }

            if (!int.TryParse(SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationCrawlCVStartAtHour), out int automationStartAtHour))
            {
                automationStartAtHour = 7;
            }

            if (!int.TryParse(SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationCrawlCVEndAtHour), out int automationEndAtHour))
            {
                automationEndAtHour = 19;
            }

            if (!now.Hour.IsBetween(automationStartAtHour, automationEndAtHour))
            {
                Logger.Info($"The current time is outside the time range configured for automatic CV creation.");
                Logger.Info($"CV will be automatically generated between {automationStartAtHour}:00 and {automationEndAtHour}:00 every day.");
                return false;
            }

            return true;
        }
        private void PreNotify()
        {
            DateTime now = DateTimeUtils.GetNow();
            var canStart = int.TryParse(SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationNoticeStartAtHour), out int startAtHour);
            var canEnd = int.TryParse(SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationNoticeEndAtHour), out int endAtHour);

            if ((canStart && canEnd) == false)
            {
                startAtHour = 10;
                endAtHour = 17;
            }

            if (now.Hour.IsBetween(startAtHour, endAtHour))
            {
                Logger.Info($"The current time is within the notification configuration period ({startAtHour} - {endAtHour}).");
                Notify();
                //InternResult.Success = StaffResult.Success = 0;
                InternResult.Success = 0;
                //InternResult.Total = StaffResult.Total = 0;
            }
        }

        private void Notify()
        {
            string notifyEmailsString = SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationNotifyToUser);
            List<string> notifyEmailsList = string.IsNullOrEmpty(notifyEmailsString)
                ? new List<string>()
                : notifyEmailsString.Split(',').Select(x => x.Trim()).ToList();

            string noticeMode = SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationNoticeMode);
            switch (noticeMode)
            {
                case "Channel":
                    string channelId = SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationNoticeChannelId);
                    string messageToChannel = BuildMessage(notifyEmailsList);
                    _komuService.NotifyToChannel(messageToChannel, channelId);
                    break;
                case "User":
                    string messageToUser = BuildMessage();
                    var discordUsers = notifyEmailsList.Select(email => CommonUtils.GetUserNameByEmail(email));
                    foreach (string discordUser in discordUsers)
                    {
                        _komuService.SendMessageToUser(discordUser, messageToUser);
                    }
                    break;
                default:
                    break;
            }
        }

        private string BuildMessage(List<string> emails = null)
        {
            var sb = new StringBuilder();
            var clientUrl = _configuration.GetValue<string>($"App:ClientRootAddress");


            if (InternResult.Success > 0)
            {
                sb.AppendLine("All new CV has automatically created successfully:");
                sb.Append($"**New CV quantity: {InternResult.Success}**");
                sb.AppendLine($": {GetTalentLink(clientUrl, UserType.Intern)}");
            }

            //if (StaffResult.Total > 0)
            //{
            //    sb.Append($"**Staff CV: {StaffResult.Success}/{StaffResult.Total}**");
            //    sb.AppendLine($": {GetTalentLink(clientUrl, UserType.Staff)}");
            //}

            if (emails != null && emails.Count > 0)
            {
                var tagNames = string.Join(", ", emails.Select(x => CommonUtils.GetDiscordTagUser(x)).ToArray());
                sb.Append($"HR: {tagNames} ");
                if (string.IsNullOrEmpty(clientUrl))
                {
                    sb.Append("please check the created CV information at Talent.");
                }
                else
                {
                    sb.Append("please check the created CV information at the attached link.");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(clientUrl))
                {
                    sb.Append("Please check the created CV information at Talent.");
                }
                else
                {
                    sb.Append("Please check the created CV information at the attached link.");
                }
            }

            return sb.ToString();
        }

        private string GetTalentLink(string clientUrl, UserType userType)
        {
            return $"{clientUrl}app/candidate/{(userType == UserType.Intern ? "intern-list" : "staff-list")}?cvStatus=20";
        }


    }
}


