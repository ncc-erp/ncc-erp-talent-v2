using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;

namespace TalentV2.Entities.NccCVs
{
    public class EmployeeSkill : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long? CVEmployeeId { get; set; }
        public long? SkillId { get; set; }
        public int ExperienceMonth { get; set; }
        public long? GroupSkillId { get; set; }
        public string SkillName { get; set; }
        public int? Level { get; set; }
        [ForeignKey(nameof(CVEmployeeId))]
        public User Cvemployee { get; set; }
        [ForeignKey(nameof(GroupSkillId))]
        public GroupSkill GroupSkill { get; set; }
        [ForeignKey(nameof(SkillId))]
        public FakeSkill Skill { get; set; }
    }
}
