using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Notifications.Message.Dtos;
using TalentV2.Notifications.Message;
using TalentV2.DomainServices.NotificationSettings;
using TalentV2.DomainServices.NotificationSettings.Dtos;
using NccCore.Extension;
using TalentV2.Authorization;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class NotificationSettingAppService: TalentV2AppServiceBase
    {
        private readonly INotificationSettingManager _notificationSettingManager;

        public NotificationSettingAppService(
            INotificationSettingManager notificationSettingManager)
        {
            _notificationSettingManager = notificationSettingManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_NotificationSettings_ViewList)]
        public async Task<GridResult<NotificationSettingDto>> GetAllPaging(GridParam param)
        {
            var query = _notificationSettingManager.IQGetAllNotificationSetting();
            var grid = await query.GetGridResult(query, param);

            if (!string.IsNullOrEmpty(param.SearchText))
                return _notificationSettingManager.ApplySearchText(grid, param.SearchText);

            return grid;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_NotificationSettings_ViewList)]
        public async Task<MessageTemplateDto> GetMessageTemplate(long notificationId)
        {
            return await _notificationSettingManager.GetMessageTemplate(notificationId);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_NotificationSettings_Preview)]
        public async Task<MessageTemplateDto> GetFakeDataById(long notificationId)
        {
            return await _notificationSettingManager.GetFakeData(notificationId);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_NotificationSettings_Update)]
        public async Task<NotificationSettingDto> Update(NotificationSettingDto notification)
        {
            return await _notificationSettingManager.UpdateNotificationSetting(notification);
        }
    }
}
