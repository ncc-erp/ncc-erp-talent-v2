using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class CreateInterviewerCVRequestDto
    {
        public long InterviewerId { get; set; }
        public long RequestCvId { get; set; }
    }
}
