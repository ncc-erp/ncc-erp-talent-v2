using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentV2.Entities
{
    public class Post : NccAuditEntity, IMayHaveTenant
    {
        [Required]
        [MaxLength(2000)]
        public string PostName { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Url { get; set; }
        [MaxLength(2000)]
        public string Type { get; set; }
        [Required]
        public DateTime PostCreationTime { get; set; }
        [MaxLength(5000)]
        public string Content { get; set; }
        public int? TenantId { get; set; }
        [Column(TypeName = "jsonb")]
        public string Metadata { get; set; }
        public long? CreatedByUser { get; set; }
        public int ApplyNumber { get; set; }
    }
}
