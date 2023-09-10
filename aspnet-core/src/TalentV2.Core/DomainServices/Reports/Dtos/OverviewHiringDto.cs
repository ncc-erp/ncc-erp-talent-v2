using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class OverviewHiringDto
    {
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public List<SubPositionStatistic> SubPositionStatistics { get; set; }
        public TotalOverviewHiring TotalOverviewHiring { get; set; }
    }
    public class SubPositionStatistic
    {
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public long QuantityHiring { get; set; }
        public long QuantityApply { get; set; }
        public long QuantityNewCV { get; set; }
        public long QuantityContacting { get; set; }
        public long QuantityPassed { get; set; }
        public long QuantityFailed { get; set; }
        public List<CVSourceStatistic> CVSourceStatistics { get; set; }
        public List<StatusStatistic> StatusStatistics { get; set; }
    }
    public class CVSourceStatistic
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long TotalCV { get; set; }
    }
    public class StatusStatistic
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long TotalCV { get; set; }
    }
}
