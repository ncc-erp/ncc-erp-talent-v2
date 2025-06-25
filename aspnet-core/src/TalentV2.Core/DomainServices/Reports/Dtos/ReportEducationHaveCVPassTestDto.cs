using System.Collections.Generic;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class ReportEducationByBranchDto<T> where T : class
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public IEnumerable<T> Educations { get; set; }
    }

    public class ReportEducationHaveCVPassTestDto
    {
        public long EducationId { get; set; }
        public string EducationName { get; set; }
        public string ColorCode { get; set; }
        public int TotalCV { get; set; }
    }

    public class CandidateQuantityInUniversity : ReportEducationHaveCVPassTestDto
    {
        public int PassCV { get; set; }
        public int PassTest { get; set; }
        public int PassInterview { get; set; }
        public int Onboard { get; set; }
        public int Other { get; set; }
    }
}
