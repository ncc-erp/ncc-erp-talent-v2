using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class EmailStatusHistory : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long CVId { get; set; }
        [ForeignKey(nameof(CVId))]
        public CV CVs { get; set; }
        public long EmailTemplateId { get; set; }
        [ForeignKey(nameof(EmailTemplateId))]
        public EmailTemplate EmailTemplate { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
