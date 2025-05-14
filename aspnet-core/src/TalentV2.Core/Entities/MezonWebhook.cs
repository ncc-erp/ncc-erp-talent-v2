using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentV2.Entities
{
    public class MezonWebhook : FullAuditedEntity<long>
    {
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(2048)]
        public string Url { get; set; }

        [Column(TypeName = "jsonb")]
        public List<string> Functions { get; set; }

        public bool IsActive { get; set; }
    }
}