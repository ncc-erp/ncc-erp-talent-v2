using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class ReportEducationHaveCVPassInterViewDto : ReportEducationHaveCVPassTestDto
    {
    }
    public class EducationPassInterViewDto
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public long CVId { get; set; }
        public string ColorCode { get; set; }
    }
}
