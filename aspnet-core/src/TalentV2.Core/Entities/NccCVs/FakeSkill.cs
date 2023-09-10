using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities.NccCVs
{
    public class FakeSkill : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long GroupSkillId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey(nameof(GroupSkillId))]
        public GroupSkill GroupSkill { get; set; }
    }
}
