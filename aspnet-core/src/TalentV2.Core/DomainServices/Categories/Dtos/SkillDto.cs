using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Categories.Dtos
{
    [AutoMapTo(typeof(Skill))]
    public class SkillDto : CategoryDto
    {
    }
}
