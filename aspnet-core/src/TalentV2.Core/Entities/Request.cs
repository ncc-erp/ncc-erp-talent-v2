using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class Request : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [MaxLength(1000)]
        public string Note { get; set; }
        public Level Level { get; set; }
        public int Quantity { get; set; }
        public StatusRequest Status { get; set; }
        public UserType UserType { get; set; }
        public long? ProjectToolRequestId { get; set; }
        public DateTime? TimeNeed { get; set; }
        public Priority Priority { get; set; }

        public long BranchId { get; set; }
        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }

        public long SubPositionId { get; set; }
        [ForeignKey(nameof(SubPositionId))]
        public SubPosition SubPosition { get; set; }

        public ICollection<RequestSkill> RequestSkills { get; set; }
        public ICollection<RequestCV> RequestCVs { get; set; }
    }
}