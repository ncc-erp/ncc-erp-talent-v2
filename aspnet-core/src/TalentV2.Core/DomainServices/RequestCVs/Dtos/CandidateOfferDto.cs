using System;
using System.Collections.Generic;
using System.Linq;
using TalentV2.Constants.Dictionary;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.ModelExtends;
using TalentV2.Utils;

namespace TalentV2.DomainServices.RequestCVs.Dtos
{
    public class CandidateOfferDto : UserInfo, IIsProjectTool
    {
        public long Id { get; set; }
        public long CVId { get; set; }
        public long SubPositionIdRequest { get; set; }
        public string SubPositionNameRequest { get; set; }
        public string Note { get; set; }
        public StatusRequest RequestStatus { get; set; }
        public string RequestStatusName { get => CommonUtils.GetEnumName(RequestStatus); }
        public RequestCVStatus RequestCVStatus { get; set; }
        public string RequestCVStatusName { get => DictionaryHelper.RequestCVStatusDict[RequestCVStatus]; }
        public Level? FinalLevel { get; set; }
        public LevelDto FinalLevelName
        {
            get
            {
                if (FinalLevel.HasValue)
                    return CommonUtils.ListLevel.FirstOrDefault(s => s.Id == FinalLevel.Value.GetHashCode());
                return new LevelDto();
            }
        }
        public Level? ApplyLevel { get; set; }
        public LevelDto ApplyLevelName
        {
            get
            {
                if (ApplyLevel.HasValue)
                    return CommonUtils.ListLevel.FirstOrDefault(s => s.Id == ApplyLevel.Value.GetHashCode());
                return new LevelDto();
            }
        }
        public Level? InterviewLevel { get; set; }
        public LevelDto InterviewLevelName
        {
            get
            {
                if (InterviewLevel.HasValue)
                    return CommonUtils.ListLevel.FirstOrDefault(s => s.Id == InterviewLevel.Value.GetHashCode());
                return new LevelDto();
            }
        }
        public float Salary { get; set; }
        public DateTime? OnboardDate { get; set; }
        public long RequestId { get; set; }
        public Level RequestLevel { get; set; }
        public LevelDto RequestLevelName { get => CommonUtils.ListLevel.FirstOrDefault(s => s.Id == RequestLevel.GetHashCode()); }
        public List<SkillDto> RequestSkills { get; set; }
        public long RequestBracnhId { get; set; }
        public string RequestBranchName { get; set; }
        public long SubRequestPositionId { get; set; }
        public string SubRequestoPositionName { get; set; }
        public string NCCEmail { get; set; }
        public string HRNote { get; set; }
        public bool IsProjectTool
        {
            get
            {
                if (ProjectToolRequestId.HasValue)
                    return true;
                return false;
            }
        }
        public long? ProjectToolRequestId { get; set; }
    }
}
