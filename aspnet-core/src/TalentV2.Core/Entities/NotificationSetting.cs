using Abp.Domain.Entities;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class NotificationSetting: NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public NotificationType Type { get; set; }
        public string WebhookUrl { get; set; }
        public string BodyMessage { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public bool IsActived { get; set; }
    }
}
