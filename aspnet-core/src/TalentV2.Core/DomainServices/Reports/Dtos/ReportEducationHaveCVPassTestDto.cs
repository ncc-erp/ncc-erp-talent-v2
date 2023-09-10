using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
