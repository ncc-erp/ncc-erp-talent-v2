using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Categories.Dtos
{
    [AutoMapTo(typeof(Education))]
    public class CreateUpdateEducationDto : CategoryDto
    {
        public string ColorCode { get; set; }
        public long EducationTypeId { get; set; }
    }
}
