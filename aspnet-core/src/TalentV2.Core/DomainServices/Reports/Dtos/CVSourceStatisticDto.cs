using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class CVSourceStatisticDto
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public List<SourcePerformance> SourcePerformances { get; set; }
    }
    public class SourcePerformance
    {
        public string SourceName { get; set; }
        public int TotalCV { get; set; }
    }
}
