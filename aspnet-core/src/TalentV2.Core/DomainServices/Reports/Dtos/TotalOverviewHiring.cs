using System.Collections.Generic;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class TotalOverviewHiring
    {
        public long TotalHiring { get; set; }
        public long TotalApply { get; set; }
        public long TotalNewCV { get; set; }
        public long TotalContacting { get; set; }
        public long TotalPassed { get; set; }
        public long TotalFailed { get; set; }
        public long[] TotalCVSources { get; set; }
        public List<TotalStatus> TotalStatuses { get; set; }
    }
    public class TotalStatus
    {
        public long TotalCV { get; set; } = 0;
        public long Id { get; set; }
    }
}
