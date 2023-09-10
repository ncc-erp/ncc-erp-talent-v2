using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class ApplyCV : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public bool IsFemale { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MaxLength(12)]
        public string Phone { get; set; }
        [Required]
        [MaxLength(15)]
        public string PositionType { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        public string Branch { get; set; }
        [Required]
        public string Avatar { get; set; }
        [Required]
        public string AttachCV { get; set; }
        public long PostId { get; set; }
        public bool? Applied { get; set; }
    }
}
