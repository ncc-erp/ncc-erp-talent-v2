using Abp.Domain.Services;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.NotificationSettings.Dtos;
using TalentV2.Notifications.Message.Dtos;

namespace TalentV2.DomainServices.NotificationSettings
{
    public interface INotificationSettingManager: IDomainService
    {
        IQueryable<NotificationSettingDto> IQGetAllNotificationSetting();
        GridResult<NotificationSettingDto> ApplySearchText(GridResult<NotificationSettingDto> grid, string searchText);
        NotificationSettingDto GetByType(NotificationType type);
        Task<MessageTemplateDto> GetMessageTemplate(long id);
        Task<MessageTemplateDto> GetFakeData(long id);
        Task<NotificationSettingDto> UpdateNotificationSetting(NotificationSettingDto input);
    }
}
