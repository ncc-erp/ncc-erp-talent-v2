using Abp.AutoMapper;
using NccCore.Anotations;
using System.ComponentModel.DataAnnotations;
using TalentV2.Entities.NccCVs;

namespace TalentV2.DomainServices.NccCVs.GroupSkills.Dtos
{
    [AutoMapTo(typeof(GroupSkill))]
    public class GroupSkillDto
    {
        public long Id { get; set; }
        [ApplySearchAttribute]
        [Required]
        public string Name { get; set; }
        public bool Default { get; set; }
    }
}
