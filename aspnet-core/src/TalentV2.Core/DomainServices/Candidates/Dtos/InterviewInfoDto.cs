using System;
using System.Collections.Generic;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Dto;

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

        public CVDto CVInfo {  get; set; }
    }

    public class InterviewerDto
    {
        public long Id { get; set; }
        public long InterviewerId { get; set; }
        public string InterviewerName { get; set; }
        public string InterviewerEmail { get; set; }
    }

    public class CVDto
    {
        public long RequestCVId { get; set; }
        public long CVId { get; set; }
        public UserType UserType { get; set; }
        public string BranchName { get; set; }
        public string PositionName { get; set; }
        public string CandidateFulName { get; set; }
        public DateTime TimeInterview { get; set; }
    }
}
