using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
    public class SubPosition : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(10)]
        public string ColorCode { get; set; }

        [Required]
        public long PositionId { get; set; }
        [ForeignKey(nameof(PositionId))]
        public Position Position { get; set; }

        public ICollection<CapabilitySetting> CapabilitySettings { get; set; }
        public ICollection<CV> CVs { get; set; }
        public ICollection<Request> Requests { get; set; }
    }
}
