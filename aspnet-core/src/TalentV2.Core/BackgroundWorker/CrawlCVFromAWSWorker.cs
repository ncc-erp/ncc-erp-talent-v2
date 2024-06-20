using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalentV2.Configuration;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.CVAutomation;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.Utils;
using TalentV2.WebServices.ExternalServices.Komu;

namespace TalentV2.BackgroundWorker
{
    public class CrawlCVFromAWSWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        public AutomationResult InternResult { get; private set; }
        public AutomationResult StaffResult { get; private set; }

        protected readonly KomuService _komuService;
        protected readonly ICVAutomationManager _cvAutomationService;
        protected readonly IConfiguration _configuration;

        public CrawlCVFromAWSWorker(
            AbpTimer timer,
            KomuService komuService,
            ICVAutomationManager cvAutomationService,
            IConfiguration configuration,
            ISettingManager settingManager) : base(timer)
        {
            _komuService = komuService;
            _cvAutomationService = cvAutomationService;
            _configuration = configuration;

            InternResult = new AutomationResult();
            StaffResult = new AutomationResult();

            if (int.TryParse(settingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationRepeatTimeInMinutes), out var repeatTimeInMinutes))
            {
                Timer.Period = 1000 * 60 * repeatTimeInMinutes;
            }
            else
            {
                Timer.Period = 1000 * 60 * 60 * 24; // default repeat each 24 hour
            }
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            Logger.Info("CrawlCVFromAWSWorker start");
            DateTime now = DateTimeUtils.GetNow();
            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
            {
                Logger.Info("Today is DayOff => stop");
                return;
            }

            AsyncHelper.RunSync(async () =>
            {
                var internResult = await _cvAutomationService.AutoCreateInternCV();
                if (internResult != null)
                {
                    InternResult.Success += internResult.Success;
                    InternResult.Total += internResult.Total;
                }
                var staffResult = await _cvAutomationService.AutoCreateStaffCV();
                if (staffResult != null)
                {
                    StaffResult.Success += staffResult.Success;
                    StaffResult.Total += staffResult.Total;
                }
            });

            bool.TryParse(SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationEnabled), out bool enableNotify);
            if (enableNotify && (InternResult.Total > 0 || StaffResult.Total > 0)) PreNotify();
        }

        private void PreNotify()
        {
            DateTime now = DateTimeUtils.GetNow();
            int startAtHour, endAtHour;
            var canStart = int.TryParse(SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationNoticeStartAtHour), out startAtHour);
            var canEnd = int.TryParse(SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationNoticeEndAtHour), out endAtHour);

            if ((canStart && canEnd) == false)
            {
                startAtHour = 10;
                endAtHour = 17;
            }

            if (now.Hour.IsBetween(startAtHour, endAtHour))
            {
                Logger.Info($"The current time is within the notification configuration period ({startAtHour} - {endAtHour}).");
                Notify();
                InternResult.Success = StaffResult.Success = 0;
                InternResult.Total = StaffResult.Total = 0;
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

            sb.AppendLine("Talent has automatically created successfully:");
            if (InternResult.Total > 0)
            {
                sb.Append($"**Intern CV: {InternResult.Success}/{InternResult.Total}**");
                sb.AppendLine($": {GetTalentLink(clientUrl, UserType.Intern)}");
            }

            if (StaffResult.Total > 0)
            {
                sb.Append($"**Staff CV: {StaffResult.Success}/{StaffResult.Total}**");
                sb.AppendLine($": {GetTalentLink(clientUrl, UserType.Staff)}");
            }

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