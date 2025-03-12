using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServices.Dto;
using TalentV2.DomainServices.ExternalCVs.Dtos;
using TalentV2.DomainServices.Requisitions.Dtos;

namespace TalentV2.DomainServices.Interviews.Dtos
{


    public class GetInterviewInfoDto : GetResultConnectDto
    {
        public List<InterviewInfoDto> InterviewInfo { get; set; }
    }
    public class InterviewInfoDto
    {
        public InterviewerDto Interviewer { get; set; }
        public string HrEmail { get; set; }
        public DateTime TimeInterview { get; set; }

    }

    public class InterviewerDto
    {
        public long Id { get; set; }
        public long InterviewerId { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewerEmail { get; set; }
    }
}
