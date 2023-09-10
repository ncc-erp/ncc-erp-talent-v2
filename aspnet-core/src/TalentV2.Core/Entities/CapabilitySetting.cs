using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using TalentV2.Constants.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TalentV2.Entities
{
    public class CapabilitySetting : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public UserType UserType { get; set; }
        [MaxLength(5000)]
        public string Note { get; set; }
        [MaxLength(5000)]
        public string GuideLine { get; set; }
        public long SubPositionId { get; set; }
        [ForeignKey(nameof(SubPositionId))]
        public SubPosition SubPosition { get; set; }
        public long CapabilityId { get; set; }
        [ForeignKey(nameof(CapabilityId))]
        public Capability Capability { get; set; }
        public int Factor { get; set; }
    }
}
