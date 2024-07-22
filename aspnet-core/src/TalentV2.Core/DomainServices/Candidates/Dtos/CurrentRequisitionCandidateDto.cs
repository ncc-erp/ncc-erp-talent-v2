using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.ModelExtends;
using TalentV2.Notifications.Mail.Dtos;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class CurrentRequisitionCandidateDto
    {
        public long Id { get; set; }
        public string CVName { get; set; }
        public CurrentRequisitionDto CurrentRequisition { get; set; }
        public List<InterviewCandidateDto> InterviewCandidate { get; set; }
        public List<CapabilityCandidateDto> CapabilityCandidate { get; set; }
        public ApplicationResultCandidateDto ApplicationResult { get; set; }
        public InterviewLevelCandidateDto InterviewLevel { get; set; }
        public InterviewedDto Interviewed { get; set; }
        public DateTime? InterviewTime { get; set; }
        public DateTime CreationTime { get; set; }
    }
    public class CurrentRequisitionDto : CreateUpdateAudit, IIsProjectTool
    {
        public long Id { get; set; }
        public Priority Priority { get; set; }
        public string PriorityName { get => CommonUtils.GetEnumName(Priority); }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public string DisplayBranchName { get; set; }
        public UserType UserType { get; set; }
        public string UserTypeName { get => CommonUtils.GetEnumName(UserType); }
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; }
        public Level Level { get; set; }
        public LevelDto LevelInfo { get => CommonUtils.ListLevel.Where(s => s.Id == Level.GetHashCode()).Select(s => s).FirstOrDefault(); }
        public List<CategoryDto> Skills { get; set; }
        public StatusRequest RequestStatus { get; set; }
        public string RequestStatusName { get => CommonUtils.GetEnumName(RequestStatus); }
        public int Quantity { get; set; }
        public DateTime? TimeNeed { get; set; }
        public long? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Note { get; set; }
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
    public class InterviewCandidateDto
    {
        public long Id { get; set; }
        public long RequestCvId { get; set; }
        public long InterviewId { get; set; }
        public string InterviewName { get; set; }
        public string EmailAddress { get; set; }
    }
    public class CapabilityCandidateDto
    {
        public long Id { get; set; }
        public long RequestCvId { get; set; }
        public long CapabilityId { get; set; }
        public string CapabilityName { get; set; } 
        public int Score { get; set; }
        public string Note { get; set; }
        public int Factor { get; set; }
        public bool FromType { get; set; }
    }
    public class ApplicationResultCandidateDto
    {
        public long CVId { get; set; }
        public RequestCVStatus Status { get; set; }
        public List<HistoryStatusesDto> HistoryStatuses { get; set; }
        public List<HistoryChangeStatusesDto> HistoryChangeStatuses { get; set; }
        public Level? ApplyLevel { get; set; }
        public string ApplyLevelName
        {
            get
            {
                if (ApplyLevel.HasValue)
                    return CommonUtils.GetEnumName(ApplyLevel.Value);
                return null;
            }
        }
        public Level? InterviewLevel { get; set; }
        public string InterviewLevelName
        {
            get
            {
                if (InterviewLevel.HasValue)
                    return CommonUtils.GetEnumName(InterviewLevel.Value);
                return null;
            }
        }
        public Level? FinalLevel { get; set; }
        public string FinalLevelName
        {
            get
            {
                if (FinalLevel.HasValue)
                    return CommonUtils.GetEnumName(FinalLevel.Value);
                return null;
            }
        }
        public float Salary { get; set; }
        public DateTime? OnboardDate { get; set; }
        public string HRNote { get; set; }
        public MailDetailDto MailDetail { get; set; }
        public string LMSInfo { get; set; }
        public string Percentage { get; set; }
        public bool? EmailSent { get; set; }
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; }
    }
    public class InterviewLevelCandidateDto
    {
        public long CVId { get; set; }
        public Level? InterviewLevel { get; set; }
        public string InterviewLevelName
        {
            get
            {
                if (InterviewLevel.HasValue)
                    return CommonUtils.GetEnumName(InterviewLevel.Value);
                return null;
            }
        }
    }
    public class InterviewedDto
    {
        public long CVId { get; set; }
        public List<int>  Score { get; set; }
        public Level? InterviewLevel { get; set; }
        public bool? Interviewed
        {
            get
            {
                if (InterviewLevel.HasValue && !Score.Contains(0))
                    return true;
                return false;
            }
        }
    }
}
