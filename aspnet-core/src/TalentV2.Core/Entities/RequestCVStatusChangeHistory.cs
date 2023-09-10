using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class RequestCVStatusChangeHistory : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public RequestCVStatus? FromStatus { get; set; }
        public DateTime TimeAt { get; set; }
        public RequestCVStatus ToStatus { get; set; }
        public long RequestCVId { get; set; }
        [ForeignKey(nameof(RequestCVId))]
        public RequestCV RequestCV { get; set; }
    }
}