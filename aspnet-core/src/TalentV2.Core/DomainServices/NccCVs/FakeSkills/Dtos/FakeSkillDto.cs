using Abp.AutoMapper;
using NccCore.Anotations;
using System.ComponentModel.DataAnnotations;
using TalentV2.Entities.NccCVs;

namespace TalentV2.DomainServices.NccCVs.FakeSkills.Dtos
{
    [AutoMapTo(typeof(FakeSkill))]
    public class FakeSkillDto
    {
        public long Id { get; set; }
        [ApplySearchAttribute]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public long GroupSkillId { get; set; }

        [ApplySearchAttribute]
        public string GroupSkillName { get; set; }
        
    }
}
