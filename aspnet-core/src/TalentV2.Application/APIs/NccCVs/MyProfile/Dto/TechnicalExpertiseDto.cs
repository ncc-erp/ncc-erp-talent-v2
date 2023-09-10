using System;
using System.Collections.Generic;
using System.Text;

namespace TalentV2.APIs.NccCVs.MyProfile.Dto
{
    public class TechnicalExpertiseDto
    {
        public long UserId { get; set; }
        public List<GroupSkillAndSkillDto> GroupSkills { get; set; }
    }

    public class GroupSkillAndSkillDto
    {
        public long? GroupSkillId { get; set; }
        public string Name { get; set; }
        public List<CVSkillDto> CVSkills { get; set; }
    }

    
    public class CVSkillDto
    {
        public long? Id { get; set; }
        public long? SkillId { get; set; }
        public string SkillName { get; set; }
        public int? Level { get; set; }
        public int? Order { get; set; }
    }
}
