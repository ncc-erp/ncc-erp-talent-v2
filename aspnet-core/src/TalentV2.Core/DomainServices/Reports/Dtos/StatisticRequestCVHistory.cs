using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class StatisticRequestCVHistory
    {
        public long SubPositionId { get; set; }
        public List<StatusStatistic> StatusStatistics { get; set; }
    }

    public class StaticCVSourcesDto
    {
        public long SubPositionId { get; set; }
        public List<CVSourceStatistic> CVSourceStatistics { get; set; }
    }
}
