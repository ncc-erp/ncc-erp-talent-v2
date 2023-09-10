using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class HistoryRequestCVDto
    {
        public long Id { get; set; }
        public RequestCVStatus Status { get; set; }
        public DateTime? OnboardDate { get; set; }
    }

    public class StatusChangeRequestCVDto
    {
        public long Id { get; set; }
        public RequestCVStatus? FromStatus { get; set; }
        public RequestCVStatus ToStatus { get; set; }
    }
}
