using Abp.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Configuration;
using TalentV2.Constants.Enum;
using TalentV2.Entities;
using TalentV2.NccCore;
using TalentV2.Notifications.Komu;
using TalentV2.Notifications.Templates.Dtos;
using TalentV2.Notifications.Templates;
using TalentV2.WebServices.ExternalServices.Komu;
using TalentV2.WebServices.ExternalServices.MezonWebhooks;
using Microsoft.EntityFrameworkCore;
using TalentV2.DomainServices.NotificationSettings;
using Amazon.Runtime.Internal.Util;
using TalentV2.Constants.Const;
using TalentV2.DomainServicesWithoutWorkScope.CandidateManager.Dtos;
using TalentV2.DomainServices.CVAutomation.Dto;
using System.Security.Policy;
using TalentV2.Notifications.MezonWebhook.Dtos;

namespace TalentV2.Notifications.MezonWebhook
{
    public class MezonWebhookNotification : IMezonWebhookNotification
    {
        private readonly IWorkScope _ws;
        private readonly ISettingManager _settingManager;
        private readonly ILogger<MezonWebhookService> _logger;
        private readonly MezonWebhookService _mezonWebhookService;
        private readonly INotificationSettingManager _notificationSettingManager;
        public MezonWebhookNotification(
            IWorkScope ws,
            ISettingManager settingManager,
            ILogger<MezonWebhookService> logger,
            MezonWebhookService mezonWebhookService,
            INotificationSettingManager notificationSettingManager)
        {
            _ws = ws;
            _settingManager = settingManager;
            _logger = logger;
            _mezonWebhookService = mezonWebhookService;
            _notificationSettingManager = notificationSettingManager;
        }

        public async Task NotifyAcceptedOrRejectedOffer(RequestCVStatus status, long requestCvId, bool isFirstAcceptedOffer = true)
        {
            var dataTemplate = _ws.GetAll<RequestCV>()
                    .Where(q => q.Id == requestCvId)
                    .Select(s => new CandidateOfferAcceptedTemplate
                    {
                        CVId = s.CVId,
                        FullName = s.CV.Name,
                        BranchName = s.CV.Branch.DisplayName,
                        OnboardDateTime = s.OnboardDate,
                        Skills = string.Join(",", s.CV.CVSkills.Select(s => s.Skill.Name).ToList()),
                        Email = s.CV.Email,
                        NCCEmail = s.CV.NCCEmail,
                        UserType = s.CV.UserType,
                        Phone = s.CV.Phone,
                        SubPositionName = s.CV.SubPosition.Name
                    }).FirstOrDefault();

            if (status == RequestCVStatus.RejectedOffer)
            {
                var notificationSetting = _notificationSettingManager.GetByType(NotificationType.RejectedOffer);
                string message = TemplateToMezon.RejectedOfferTemplate(dataTemplate, notificationSetting.Id);

                _mezonWebhookService.NotifyToWebhookUrl(message, notificationSetting.WebhookUrl);
            }
            else
            {
                var notificationSetting = _notificationSettingManager.GetByType(NotificationType.AcceptedOffer);
                string message = TemplateToMezon.AcceptedOfferTemplate(dataTemplate, notificationSetting.Id);

                _mezonWebhookService.NotifyToWebhookUrl(message, notificationSetting.WebhookUrl);
            }
        }

        public async Task NotifyUpdatedPersonalInfoTemplate(long requestCvId)
        {
            var dataTemplate = await _ws.GetAll<RequestCV>()
                    .Where(q => q.Id == requestCvId)
                    .Select(s => new CandidateOfferAcceptedTemplate
                    {
                        CVId = s.CVId,
                        FullName = s.CV.Name,
                        BranchName = s.CV.Branch.DisplayName,
                        OnboardDateTime = s.OnboardDate,
                        Skills = string.Join(",", s.CV.CVSkills.Select(s => s.Skill.Name).ToList()),
                        Email = s.CV.Email,
                        NCCEmail = s.CV.NCCEmail,
                        UserType = s.CV.UserType,
                        Phone = s.CV.Phone,
                    }).FirstOrDefaultAsync();

            var notificationSetting = _notificationSettingManager.GetByType(NotificationType.UpdatedPersonalInfo);
            string message = TemplateToMezon.UpdatedPersonalInfoTemplate(dataTemplate, notificationSetting.Id);

            _mezonWebhookService.NotifyToWebhookUrl(message, notificationSetting.WebhookUrl);
        }

        public void NotifyNoticeInterviewToMezonChannel(NoticeInterviewDto input, string feUrl, bool isSchedule)
        {
            NotificationType type;
            if (!isSchedule && input.InterviewLevel == null)
                type = NotificationType.ChannelNotice_CandidateEvaluationAndLevel;
            else if (!isSchedule)
                type = NotificationType.ChannelNotice_CandidateEvaluation;
            else
                type = NotificationType.ChannelNotice_InterviewRemind;

            var notificationSetting = _notificationSettingManager.GetByType(type);

            string message = TemplateToMezon.NoticeInterviewToChannelTemplate(input, feUrl, notificationSetting);
            _mezonWebhookService.NotifyToWebhookUrl(message, notificationSetting.WebhookUrl);
        }

        public void NotifyCrawlCVToMezonChannel(
            CrawlCVDto crawlDto,
            string clientUrl,
            List<string> emails)
        {
            var notificationSetting = _notificationSettingManager.GetByType(NotificationType.CrawlCV_MessageToChannel);

            string message = TemplateToMezon.CrawlCVToChannelTemplate(notificationSetting, crawlDto, clientUrl, emails);
            _mezonWebhookService.NotifyToWebhookUrl(message, notificationSetting.WebhookUrl);
        }
    }
}
