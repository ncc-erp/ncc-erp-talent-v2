using Abp.Domain.Uow;
using Abp.ObjectMapping;
using Abp.UI;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Constants.Enum.NccCVs;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.DomainServices.NotificationSettings.Dtos;
using TalentV2.DomainServices.Reports.Dtos;
using TalentV2.DomainServices.RequestCVs.Dtos;
using TalentV2.DomainServicesWithoutWorkScope.CandidateManager.Dtos;
using TalentV2.Entities;
using TalentV2.NccCore;
using TalentV2.Notifications.Mail.Dtos;
using TalentV2.Notifications.Message.Dtos;
using TalentV2.Notifications.MezonWebhook;
using TalentV2.Notifications.MezonWebhook.Dtos;
using TalentV2.Notifications.Templates.Dtos;
using TalentV2.Utils;
using TalentV2.WebServices.ExternalServices.MezonWebhooks;

namespace TalentV2.DomainServices.NotificationSettings
{
    public class NotificationSettingManager: INotificationSettingManager
    {
        private readonly IWorkScope _workScope;
        public IUnitOfWorkManager _unitOfWorkManager { get; private set; }

        private readonly MezonWebhookService _mezonWebhookService;

        private IObjectMapper _objectMapper { get; set; }

        public NotificationSettingManager(
            IWorkScope workScope,
            IUnitOfWorkManager unitOfWorkManager,
            MezonWebhookService mezonWebhookService,
            IObjectMapper objectMapper)
        {  
            _workScope = workScope;
            _unitOfWorkManager = unitOfWorkManager;
            _mezonWebhookService = mezonWebhookService;
            _objectMapper = objectMapper;
        }
        public IQueryable<NotificationSettingDto> IQGetAllNotificationSetting()
        {
            var qallNotificationSetting = from notification in _workScope.GetAll<NotificationSetting>()
                                           select new NotificationSettingDto
                                           {
                                               Id = notification.Id,
                                               TenantId = notification.TenantId,
                                               WebhookUrl = notification.WebhookUrl,
                                               BodyMessage = notification.BodyMessage,
                                               Type = notification.Type,
                                               Description = notification.Description,
                                               IsActived = notification.IsActived,
                                           };
            return qallNotificationSetting;
        }

        public GridResult<NotificationSettingDto> ApplySearchText(GridResult<NotificationSettingDto> grid, string searchText)
        {
            List<NotificationSettingDto> resultList = new List<NotificationSettingDto>();

            foreach (var item in grid.Items)
            {
                string typeName = item.TypeName.ToLower();
                string text = searchText.ToLower();
                if (typeName.Contains(text))
                    resultList.Add(item);
            }
            return new GridResult<NotificationSettingDto>(resultList, resultList.Count);
        }

        public NotificationSettingDto GetByType(NotificationType type)
        {
            return IQGetAllNotificationSetting().FirstOrDefault(x => x.Type == type);
        }

        public async Task<MessageTemplateDto> GetMessageTemplate(long id)
        {
            var template = await _workScope.GetAsync<NotificationSetting>(id);

            if (template == null)
                throw new UserFriendlyException("Notification Setting has not existed!");

            var data = GetResultTemplateData(template.Type);

            return new MessageTemplateDto
            {
                Type = template.Type,
                BodyMessage = template.BodyMessage,
                PropertiesSupport = data?.PropertiesSupport,
            };
        }

        public async Task<MessageTemplateDto> GetFakeData(long id)
        {
            var template = await _workScope.GetAsync<NotificationSetting>(id);

            if (template == null)
                throw new UserFriendlyException("Notification Setting has not existed!");

            var templateDto = _objectMapper.Map<NotificationSettingDto>(template);
            var fakeData = GetResultTemplateData(template.Type)?.Result;

            return new MessageTemplateDto
            {
                Type = template.Type,
                BodyMessage = TemplateToMezon.GeneratePreviewMessage(fakeData, templateDto),
            };
        }

        private dynamic GetResultTemplateData(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.AcceptedOffer:
                case NotificationType.RejectedOffer:
                case NotificationType.UpdatedPersonalInfo:
                    return GetHRMessageData();
                case NotificationType.ChannelNotice_InterviewRemind:
                case NotificationType.ChannelNotice_CandidateEvaluation:
                case NotificationType.ChannelNotice_CandidateEvaluationAndLevel:
                case NotificationType.UserNotice_InterviewRemind:
                case NotificationType.UserNotice_CandidateEvaluation:
                case NotificationType.UserNotice_CandidateEvaluationAndLevel:
                    return GetInterviewWorkerData();
                case NotificationType.CrawlCV_MessageToChannel:
                case NotificationType.CrawlCV_MessageToUser:
                    return GetCrawlCVWorkerData();
                default:
                    return null;
            }
        }

        private ResultMessageTemplate<CandidateOfferAcceptedResult> GetHRMessageData()
        {
            var fakeData = new CandidateOfferAcceptedResult
            {
                FullName = "Nguyen Van A",
                BranchName = "HN",
                NCCEmail = "",
                Email = "nguyenvana@email.com",
                OnboardDateTime = DateTime.Now,
                Phone = "0912845678",
                Skills = "",
                SubPositionName = "Java Developer",
                UserType = UserType.Intern,
            };
            return new ResultMessageTemplate<CandidateOfferAcceptedResult>
            {
                Result = fakeData
            };
        }

        private ResultMessageTemplate<NoticeInterviewResult> GetInterviewWorkerData()
        {
            var fakeData = new NoticeInterviewResult
            {
                UserType = UserType.Intern,
                BranchName = "HN",
                PositionName = "Developer",
                CandidateFulName = "Nguyen Van A",
                TimeInterview = DateTime.Now,
                InterviewerEmails = new List<string>() { "an.nguyenvan@ncc.asia", "an.nguyenvan2@ncc.asia" },
                InterviewLevel = Level.Intern_2,
            };
            return new ResultMessageTemplate<NoticeInterviewResult>
            {
                Result = fakeData
            };
        }

        private ResultMessageTemplate<CrawlCVResult> GetCrawlCVWorkerData()
        {
            var fakeData = new CrawlCVResult
            {
                InternCVQuantity = "0",
                StaffCVQuantity = "1",
            };

            return new ResultMessageTemplate<CrawlCVResult>
            {
                Result = fakeData
            };
        }

        public async Task<NotificationSettingDto> UpdateNotificationSetting(NotificationSettingDto input)
        {
            NotificationSetting notification = await _workScope.GetAsync<NotificationSetting>(input.Id);
            if (notification == null)
            {
                throw new UserFriendlyException("Notification Setting has not already existed!");
            }

            notification.WebhookUrl = input.WebhookUrl;
            notification.IsActived = input.IsActived;
            notification.BodyMessage = input.BodyMessage;

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                await _workScope.UpdateAsync(notification);
                uow.Complete();
            }

            return IQGetAllNotificationSetting()
                .FirstOrDefault(n => n.Id == notification.Id); 
        }
    }
}