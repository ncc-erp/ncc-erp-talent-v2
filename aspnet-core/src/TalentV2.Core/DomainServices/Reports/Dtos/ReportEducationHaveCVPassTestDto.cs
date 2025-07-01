using System.Collections.Generic;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class ReportEducationHaveCVPassTestDto
    {
        public long EducationId { get; set; }
        public string EducationName { get; set; }
        public string ColorCode { get; set; }
        public int TotalCV { get; set; }
    }
}
