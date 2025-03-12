using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServices.Requisitions.Dtos;

namespace TalentV2.DomainServices.Interviews.Dtos
{
    public class InterviewInfoDto
    {
        public InterviewerDto Interviewer { get; set; }
        public string HrEmail { get; set; }
        public DateTime TimeInterview { get; set; }

        public DateTime InterviewIndate { get; set; }
    }

    public class InterviewerDto
    {
        public long Id { get; set; }
        public long InterviewerId { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewerEmail { get; set; }
    }
}
