using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class HistoryStatusesDto
    {
        public RequestCVStatus Status { get; set; }
        public string StatusName { get => CommonUtils.GetEnumName(Status); }
        public DateTime? TimeAt { get; set; }
    }

    public class HistoryChangeStatusesDto
    {
        public RequestCVStatus ToStatus { get; set; }
        public string ToStatusName { get => CommonUtils.GetEnumName(ToStatus); }
        public RequestCVStatus? FromStatus { get; set; }
        public string FromStatusName { get => FromStatus.HasValue ? CommonUtils.GetEnumName(FromStatus.Value) : ""; }
        public DateTime? TimeAt { get; set; }
    }
}
