using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Dictionary;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.NotificationSettings.Dtos
{
    [AutoMap(typeof(NotificationSetting))]
    public class NotificationSettingDto : EntityDto<long>
    {
        public int? TenantId { get; set; }
        public NotificationType Type { get; set; }
        public string TypeName { get => DictionaryHelper.NotificationTypeDict[Type]; }
        public string WebhookUrl { get; set; }
        public string BodyMessage { get; set; }
        public string Description { get; set; }
        public bool IsActived { get; set; }
    }
}
