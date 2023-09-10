using System;
using System.Collections.Generic;
using System.Text;

namespace TalentV2.APIs.NccCVs.MyProfile.Dto
{
    public class TechnicalExpertiseInputDto
    {
        public long? Id { get; set; }
        public long? CVEmployeeId { get; set; }
        public long? SkillId { get; set; }
        public long GroupSkillId { get; set; }
        public string SkillName { get; set; }
        public int? Level { get; set; }
    }
}
