using Abp.Dependency;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NccCore.Extension;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.DomainServices.NotificationSettings;
using TalentV2.DomainServices.NotificationSettings.Dtos;
using TalentV2.DomainServicesWithoutWorkScope.CandidateManager.Dtos;
using TalentV2.Entities;
using TalentV2.Notifications.Komu.Dtos;
using TalentV2.Notifications.Message;
using TalentV2.Notifications.MezonWebhook.Dtos;
using TalentV2.Notifications.Templates.Dtos;
using TalentV2.Utils;

namespace TalentV2.Notifications.MezonWebhook
{
    public static class TemplateToMezon
    {
        private static readonly INotificationSettingManager _notificationSettingManager;
        static TemplateToMezon()
        {
            _notificationSettingManager = IocManager.Instance.Resolve<NotificationSettingManager>();
        }

        public static string GenerateMessage<T>(T input, long notificationId) where T : class
        {
            var template = _notificationSettingManager.IQGetAllNotificationSetting()
                .FirstOrDefault(x => x.Id == notificationId);

            var stringBuilder = new StringBuilder(template.BodyMessage);

            string[] properties = typeof(T).GetProperties().Select(s => s.Name).ToArray();

            foreach (var property in properties)
            {
                string placeholder = "{{" + property + "}}";
                //check if property is a IEnumerable
                var propertyType = typeof(T).GetProperty(property).PropertyType;
                if (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
                    continue;

                string data = typeof(T).GetProperty(property)?.GetValue(input)?.ToString();
                stringBuilder.Replace(placeholder, data);
            }
            stringBuilder.Replace("<p>", "");
            stringBuilder.Replace("</p>", "\n");

            return stringBuilder.ToString();
        }

        public static string GeneratePreviewMessage(dynamic input, NotificationSettingDto notiSetting)
        {
            StringBuilder previewMessage = new StringBuilder();
            var emails = new List<string>() { "an.nguyenvan@ncc.asia", "an.nguyenvan2@ncc.asia" };
            string baseUrl = "http://localhost:4200/";

            switch (notiSetting.Type)
            {
                case NotificationType.AcceptedOffer:
                    previewMessage.Append(AcceptedOfferTemplate(input, notiSetting.Id, false));
                    break;
                case NotificationType.RejectedOffer:
                    previewMessage.Append(RejectedOfferTemplate(input, notiSetting.Id));
                    break;
                case NotificationType.UpdatedPersonalInfo:
                    previewMessage.Append(UpdatedPersonalInfoTemplate(input, notiSetting.Id));
                    break;
                case NotificationType.ChannelNotice_InterviewRemind:
                case NotificationType.ChannelNotice_CandidateEvaluation:
                case NotificationType.ChannelNotice_CandidateEvaluationAndLevel:
                    previewMessage.Append(NoticeInterviewToChannelTemplate(input, baseUrl, notiSetting));
                    break;
                case NotificationType.UserNotice_InterviewRemind:
                    previewMessage.Append(NoticeInterviewToUserTemplate(input, baseUrl, true));
                    break;
                case NotificationType.UserNotice_CandidateEvaluation:
                    previewMessage.Append(NoticeInterviewToUserTemplate(input, baseUrl, false));
                    break;
                case NotificationType.UserNotice_CandidateEvaluationAndLevel:
                    input.InterviewLevel = null;
                    previewMessage.Append(NoticeInterviewToUserTemplate(input, baseUrl, false));
                    break;
                case NotificationType.CrawlCV_MessageToChannel:
                    previewMessage.Append(CrawlCVToChannelTemplate(notiSetting, input, baseUrl, emails));
                    break;
                case NotificationType.CrawlCV_MessageToUser:
                    previewMessage.Append(CrawlCVToUserTemplate(input, baseUrl));
                    break;
            }
            previewMessage.Insert(0, "<div>");
            previewMessage.Replace("\n", "</div><div>");
            previewMessage.Append("</div>");

            return previewMessage.ToString();
        }

        public static string RejectedOfferTemplate(
            CandidateOfferAcceptedTemplate input,
            long notificationId)
        {
            return GenerateMessage(input, notificationId);
        }

        public static string AcceptedOfferTemplate(
            CandidateOfferAcceptedTemplate input, 
            long notificationId,
            bool isFirstAcceptedOffer = true)
        {
            StringBuilder message = new StringBuilder(GenerateMessage(input, notificationId));
            
            var resultDto = new CandidateOfferAcceptedResult();
            string CVStatus = "{{" + nameof(resultDto.CVStatus) + "}}";

            if (!isFirstAcceptedOffer)
                message.Replace(CVStatus, " **[UPDATE]**");
            return message.ToString();
        }

        public static string UpdatedPersonalInfoTemplate(
            CandidateOfferAcceptedTemplate input,
            long notificationId)
        {
            return GenerateMessage(input, notificationId);
        }

        public static string NoticeInterviewToChannelTemplate(
            NoticeInterviewDto input,
            string feUrl,
            NotificationSettingDto notificationSetting)
        {
            StringBuilder sb = new StringBuilder();
            
            var resultDto = new NoticeInterviewResult();
            string InterviewerEmails = "{{" + nameof(resultDto.InterviewerEmails) + "}}";
            string CVLink = "{{" + nameof(resultDto.CVLink) + "}}";

            sb.Append(GenerateMessage(input, notificationSetting.Id));
            sb.Replace(InterviewerEmails, string.Join(", ", input.InterviewerEmails.Select(i => CommonUtils.GetMezonTagUser(i))));
            sb.Replace(input.TimeInterview.ToString(), DateTimeUtils.ToddMMyyyyHHmm(input.TimeInterview));
            
            string userTypeName = input.UserType == UserType.Intern ? "intern-list" : "staff-list";
            string userTypeId = ((int)input.UserType).ToString();
            string cvlinkValue = $"{feUrl}app/candidate/{userTypeName}/{input.CVId}?userType={userTypeId}&tab=3";
            sb.Replace(CVLink, cvlinkValue);

            return sb.ToString();
        }

        public static string NoticeInterviewToUserTemplate(
            NoticeInterviewDto input,
            string feUrl,
            bool isSchedule)
        {
            NotificationType type;

            if (!isSchedule && input.InterviewLevel == null)
                type = NotificationType.UserNotice_CandidateEvaluationAndLevel;
            else if (!isSchedule)
                type = NotificationType.UserNotice_CandidateEvaluation;
            else
                type = NotificationType.UserNotice_InterviewRemind;

            StringBuilder sb = new StringBuilder();
            var notificationSetting = _notificationSettingManager.GetByType(type);
            
            var resultDto = new NoticeInterviewResult();
            string CVLink = "{{" + nameof(resultDto.CVLink) + "}}";

            sb.Append(GenerateMessage(input, notificationSetting.Id));
            sb.Replace(input.TimeInterview.ToString(), DateTimeUtils.ToddMMyyyyHHmm(input.TimeInterview));

            string userTypeName = input.UserType == UserType.Intern ? "intern-list" : "staff-list";
            string userTypeId = ((int)input.UserType).ToString();
            string cvlinkValue = $"{feUrl}app/candidate/{userTypeName}/{input.CVId}?userType={userTypeId}&tab=3";
            sb.Replace(CVLink, cvlinkValue);

            return sb.ToString();
        }

        public static string CrawlCVToChannelTemplate(
            NotificationSettingDto notificationSetting,
            CrawlCVDto input,
            string clientUrl,
            List<string> emails)
        {
            StringBuilder sb = new StringBuilder();
            
            var resultDto = new CrawlCVResult();
            string InternLink = "{{" + nameof(resultDto.InternLink) + "}}";
            string StaffLink = "{{" + nameof(resultDto.StaffLink) + "}}";
            string TagNames = "{{" + nameof(resultDto.TagNames) + "}}";
            string LinkOrTalent = "{{" + nameof(resultDto.LinkOrTalent) + "}}";

            sb.Append(GenerateMessage(input, notificationSetting.Id));
            string internLinkValue = $"{clientUrl}app/candidate/intern-list?cvStatus=20";
            string staffLinkValue = $"{clientUrl}app/candidate/staff-list?cvStatus=20";
            sb.Replace(InternLink, internLinkValue);
            sb.Replace(StaffLink, staffLinkValue);
            sb.Replace(TagNames, string.Join(", ", emails.Select(x => CommonUtils.GetMezonTagUser(x)).ToArray()));
            sb.Replace(LinkOrTalent, string.IsNullOrEmpty(clientUrl) ? "Talent" : "the attached link");

            return sb.ToString();
        }

        public static string CrawlCVToUserTemplate(
            CrawlCVDto input,
            string clientUrl)
        {
            StringBuilder sb = new StringBuilder();
            var notificationSetting = _notificationSettingManager.GetByType(NotificationType.CrawlCV_MessageToUser);
            
            var resultDto = new CrawlCVResult();
            string InternLink = "{{" + nameof(resultDto.InternLink) + "}}";
            string StaffLink = "{{" + nameof(resultDto.StaffLink) + "}}";
            string LinkOrTalent = "{{" + nameof(resultDto.LinkOrTalent) + "}}";

            sb.Append(GenerateMessage(input, notificationSetting.Id));
            string internLinkValue = $"{clientUrl}app/candidate/intern-list?cvStatus=20";
            string staffLinkValue = $"{clientUrl}app/candidate/staff-list?cvStatus=20";
            sb.Replace(InternLink, internLinkValue);
            sb.Replace(StaffLink, staffLinkValue);
            sb.Replace(LinkOrTalent, string.IsNullOrEmpty(clientUrl) ? "Talent" : "the attached link");

            return sb.ToString();
        }
    }
}
