using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class CandidateFilterPaging : GridParam
    {
        public RequestCVStatus? RequestCVStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<long> SkillIds { get; set; }
        public bool IsAndCondition { get; set; }
        public ProcessCVStatus? ProcessCVStatus { get; set; }
        public RequestCVStatus? FromStatus { get; set; }
        public RequestCVStatus? ToStatus { get; set; }
    }
}
