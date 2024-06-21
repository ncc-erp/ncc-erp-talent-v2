using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class CV : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public string Email { get; set; }
        [MaxLength(12)]
        public string Phone { get; set; }
        [MaxLength(250)]
        public string Address { get; set; }
        public long? ReferenceId { get; set; }
        public DateTime? Birthday { get; set; }
        [MaxLength(250)]
        public string Avatar { get; set; }
        public UserType UserType { get; set; }
        public bool IsFemale { get; set; }
        public string LinkCV { get; set; }
        [MaxLength(100)]
        public string NCCEmail { get; set; }
        [MaxLength(1000)]
        public string Note { get; set; }
        public CVStatus CVStatus { get; set; }

        public long? CVSourceId { get; set; }
        [ForeignKey(nameof(CVSourceId))]
        public CVSource CVSource { get; set; }

        public long BranchId { get; set; }
        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }

        public long SubPositionId { get; set; }
        [ForeignKey(nameof(SubPositionId))]
        public SubPosition SubPosition { get; set; }

        public ICollection<CVSkill> CVSkills { get; set; }
        public ICollection<CVEducation> CVEducations { get; set; }
        public ICollection<RequestCV> RequestCVs { get; set; }
        public ICollection<EmailStatusHistory> EmailStatusHistories { get; set; }
        public bool? isClone { get; set; }
    }
}
