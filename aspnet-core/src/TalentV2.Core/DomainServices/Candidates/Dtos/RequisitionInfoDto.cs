using System;
using System.Collections.Generic;
using System.Linq;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.ModelExtends;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class RequisitionInfoDto : IIsProjectTool
    {
        public long Id { get; set; }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public string DisplayBranchName { get; set; }
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public string Note { get; set; }
        public Level Level { get; set; }
        public LevelDto LevelInfo { get => CommonUtils.ListLevel.FirstOrDefault(s => s.Id == Level.GetHashCode()); }
        public List<CategoryDto> SkillRequests { get; set; }
        public StatusRequest RequestStatus { get; set; }
        public string RequestStatusName { get => CommonUtils.GetEnumName(RequestStatus); }
        public RequestCVStatus RequestCVStatus { get; set; }
        public string RequestCVStatusName { get => CommonUtils.GetEnumName(RequestCVStatus); }
        public DateTime? LastModifiedTime { get; set; }
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
        public List<RequestCVCapabilityResultDto> CapabilityResults { get; set; }
        public Level? ApplyLevel { get; set; }
        public Level? InterviewLevel { get; set; }
        public Level? FinalLevel { get; set; }
        public DateTime? InterviewTime;
    }
}
