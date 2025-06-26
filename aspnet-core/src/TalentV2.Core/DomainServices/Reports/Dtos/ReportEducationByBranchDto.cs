using System.Collections.Generic;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class ReportEducationByBranchDto<T> where T : class
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public IEnumerable<T> Educations { get; set; }
    }
}
