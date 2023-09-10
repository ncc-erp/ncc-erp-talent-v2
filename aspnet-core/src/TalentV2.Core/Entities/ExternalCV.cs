using Abp.Domain.Entities;
using Castle.MicroKernel.SubSystems.Conversion;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class ExternalCV : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [Required]
        public string ExternalId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public string Email { get; set; }
        [MaxLength(12)]
        public string Phone { get; set; }
        [MaxLength(500)]
        public string Address { get; set; }
        [MaxLength(100)]
        public string ReferenceName { get; set; }
        public DateTime? Birthday { get; set; }
        [MaxLength(250)]
        public string Avatar { get; set; }
        public string UserTypeName { get; set; }
        public bool IsFemale { get; set; }
        public string LinkCV { get; set; }
        [MaxLength(100)]
        public string NCCEmail { get; set; }
        [MaxLength(1000)]
        public string Note { get; set; }
        [MaxLength(100)]
        public string CVSourceName { get; set; }
        [MaxLength(100)]
        public string BranchName { get; set; }
        [MaxLength(100)]
        public string PositionName { get; set; }
        [Column(TypeName = "jsonb")]
        public string Metadata { get; set; }
    }
}
