using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.ModelExtends;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Requisitions.Dtos
{
    public class CVRequisitionDto : UserInfo, IIsProjectTool
    {
        public long Id { get; set; }
        public long RequestId { get; set; }
        public long CVId { get; set; }
        public List<SkillDto> Skills { get; set; }
        public DateTime ApplyTime { get; set; }
        public List<InterviewDto> Interviews { get; set; }
        public DateTime? InterviewTime { get; set; }
        public RequestCVStatus RequestCVStatus { get; set; }
        public string RequestCVStatusName { get => CommonUtils.GetEnumName(RequestCVStatus); }
        public Level? ApplyLevel { get; set; }
        public LevelDto ApplyLevelInfo
        {
            get
            {
                if (!ApplyLevel.HasValue) return null;
                return CommonUtils.ListLevel.FirstOrDefault(s => s.Id == ApplyLevel.Value.GetHashCode());
            }
        }
        public Level? InterviewLevel { get; set; }
        public LevelDto InterviewLevelInfo
        {
            get
            {
                if (!InterviewLevel.HasValue) return null;
                return CommonUtils.ListLevel.FirstOrDefault(s => s.Id == InterviewLevel.Value.GetHashCode());
            }
        }
        public Level? FinalLevel { get; set; }
        public LevelDto FinalLevelInfo
        {
            get
            {
                if (!FinalLevel.HasValue) return null;
                return CommonUtils.ListLevel.FirstOrDefault(s => s.Id == FinalLevel.Value.GetHashCode());
            }
        }
        public DateTime? OnboardDate { get; set; }
        public string HrNote { get; set; }
        public long RequestBracnhId { get; set; }
        public string RequestBranchName { get; set; }
        public long RequestSubPositionId { get; set; }
        public string RequestSubPositionName { get; set; }
        public StatusRequest RequestStatus { get; set; }
        public string RequestStatusName { get => CommonUtils.GetEnumName(RequestStatus); }
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
        public ProcessCVStatus ProcessCVStatus { get; set; }
    }
    public class InterviewDto
    {
        public long Id { get; set; }
        public long InterviewerId { get; set; }
        public string InterviewerName { get; set; }
    }
}
