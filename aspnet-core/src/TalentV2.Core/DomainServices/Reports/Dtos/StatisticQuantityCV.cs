using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class StatisticQuantityCV
    {
        public long SubPositionId { get; set; }
        public long QuantityApply { get; set; }
        public long QuantityNewCV { get; set; }
        public long QuantityContacting { get; set; }
        public long QuantityPassed { get; set; }
        public long QuantityFailed { get; set; }
    }
}
