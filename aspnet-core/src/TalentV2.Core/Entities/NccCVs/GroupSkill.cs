using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities.NccCVs
{
    public class GroupSkill : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }

        public ICollection<FakeSkill> Skills { get; set; }
        public bool Default { get; set; }
    }
}
