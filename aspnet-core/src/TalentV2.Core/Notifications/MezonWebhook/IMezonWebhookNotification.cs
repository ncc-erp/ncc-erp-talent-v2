using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.DomainServicesWithoutWorkScope.CandidateManager.Dtos;
using TalentV2.Notifications.Komu.Dtos;
using TalentV2.Notifications.MezonWebhook.Dtos;

namespace TalentV2.Notifications.MezonWebhook
{
    public interface IMezonWebhookNotification : ITransientDependency
    {
        Task NotifyAcceptedOrRejectedOffer(RequestCVStatus status, long requestCvId, bool isFirstAcceptedOffer = true);
        Task NotifyUpdatedPersonalInfoTemplate(long requestCvId);
        void NotifyNoticeInterviewToMezonChannel(NoticeInterviewDto input, string feUrl, bool isSchedule);
        void NotifyCrawlCVToMezonChannel(CrawlCVDto crawlResult, string clientUrl, List<string> emails = null);
    }
}
