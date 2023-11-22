using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class ReportEducationHaveCVOnboardDto : ReportEducationHaveCVPassTestDto
    {
    }

    public class EducationDto
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public long CVId { get; set; }
        public string ColorCode { get; set; }
    }
}
