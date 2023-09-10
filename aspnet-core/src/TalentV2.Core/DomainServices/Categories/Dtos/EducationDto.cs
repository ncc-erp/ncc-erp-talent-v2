using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Categories.Dtos
{
    public class EducationDto : CategoryDto
    {
        public long EducationTypeId { get; set; }
        public string EducationTypeName { get; set; }
        public string ColorCode { get; set; }
    }
}
