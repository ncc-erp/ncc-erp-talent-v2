using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.Entities;
using TalentV2.ModelExtends;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class SkillCandidateDto : CreateUpdateAudit
    {
        public long Id { get; set; }
        public long CVId { get; set; }
        public long SkillId { get; set;}
        public string SkillName { get; set; }
        public Level LevelSkill { get; set; }
        public LevelDto LevelInfo { get => CommonUtils.ListLevel.FirstOrDefault(s => s.Id == LevelSkill.GetHashCode()); }
        public string Note { get; set; }
    }

    [AutoMapTo(typeof(CVSkill))]
    public class CreateUpdateSkillCandidateDto
    {
        public long Id { get; set; }
        public long CVId { get; set; }
        public long SkillId { get; set; }
        public Level Level { get; set; }
        public string Note { get; set; }
    }
}
