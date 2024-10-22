using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
    public class MezonWebhook : FullAuditedEntity<long>
    {
        [MaxLength(32)]
        public string Name { get; set; }
        [MaxLength(2048)]
        public string Url { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(2048)]
        public string Destination { get; set; }

    }
}
