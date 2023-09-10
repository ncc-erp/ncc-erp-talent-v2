using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Notifications.Komu.Dtos;

namespace TalentV2.Notifications.Komu
{
    public interface IKomuNotification : ITransientDependency
    {
        Task NotifyUpdatedPersonalInfoTemplate(long requestCvId);
        void NotifyAcceptedOrRejectedOffer(RequestCVStatus status, long requestCvId, bool isFirstAcceptedOffer = true);
        Task NotifyRequestFromProject(NotificationRequestFromProject input);
    }
}
