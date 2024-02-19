using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class RequesitionCVDto
    {
        public long CvId { get; set; }
        public long RequestId { get; set; }
        public long? CurrentRequestId { get; set; }
        public bool IsPresentForHr { get; set; }
        public RequestCVStatus? RequestCVStatus { get; set; }
    }

    public class CandidateRequestCVDto
    {
        public long CvId { get; set; }
        public long RequestId { get; set; }
        public RequestCVStatus? RequestCVStatus { get; set; }
    }
}
