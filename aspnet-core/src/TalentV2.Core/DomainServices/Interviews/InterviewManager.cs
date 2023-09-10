using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.DomainServices.Interview.Dtos;
using TalentV2.DomainServices.Requisitions.Dtos;
using TalentV2.Entities;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Interviews
{
    public class InterviewManager : BaseManager, IInterviewManager
    {
        public InterviewManager() { }

        public IQueryable<CVRequisitionDto> IQCVsByInterviewer()
        {
            var interviewStatusIds = CommonUtils.ListInterviewStatus.Select(s => (RequestCVStatus)s.Id).ToList();
            var query = WorkScope.GetAll<RequestCV>()
                .Where(rqCv => !rqCv.Request.IsDeleted && interviewStatusIds.Contains(rqCv.Status))
                .Select(rqCv => new CVRequisitionDto
                {
                    CVId = rqCv.CVId,
                    RequestId = rqCv.RequestId,
                    Id = rqCv.Id,
                    FullName = rqCv.CV.Name,
                    Email = rqCv.CV.Email,
                    Phone = rqCv.CV.Phone,
                    PathAvatar = rqCv.CV.Avatar,
                    PathLinkCV = rqCv.CV.LinkCV,
                    UserType = rqCv.CV.UserType,
                    IsFemale = rqCv.CV.IsFemale,
                    CvStatus = rqCv.CV.CVStatus,
                    BranchId = rqCv.CV.BranchId,
                    BranchName = rqCv.CV.Branch.Name,
                    DisplayBranchName = rqCv.CV.Branch.DisplayName,
                    SubPositionId = rqCv.CV.SubPositionId,
                    SubPositionName = rqCv.CV.SubPosition.Name,
                    ApplyTime = rqCv.CreationTime,
                    Interviews = rqCv.RequestCVInterviews
                            .Select(s => new InterviewDto
                            {
                                Id = s.Id,
                                InterviewerId = s.InterviewId,
                                InterviewerName = s.Interview.FullName
                            }).ToList(),
                    Skills = rqCv.CV.CVSkills.Select(s => new SkillDto
                    {
                        Id = s.Id,
                        Name = s.Skill.Name
                    }).ToList(),
                    InterviewTime = rqCv.InterviewTime,
                    ApplyLevel = rqCv.ApplyLevel,
                    FinalLevel = rqCv.FinalLevel,
                    InterviewLevel = rqCv.InterviewLevel,
                    OnboardDate = rqCv.OnboardDate,
                    RequestCVStatus = rqCv.Status,
                    CreationTime = rqCv.CreationTime,
                    CreatorName = rqCv.CreatorUserId.HasValue ?
                                    rqCv.CreatorUser.FullName :
                                    "",
                    LastModifiedTime = rqCv.LastModificationTime,
                    LastModifiedName = rqCv.LastModifierUserId.HasValue ?
                                        rqCv.LastModifierUser.FullName :
                                        "",
                    HrNote = rqCv.HRNote,
                    RequestBracnhId = rqCv.Request.BranchId,
                    RequestBranchName = rqCv.Request.Branch.Name,
                    RequestSubPositionName = rqCv.Request.SubPosition.Name,
                    RequestSubPositionId = rqCv.Request.SubPositionId,
                    RequestStatus = rqCv.Request.Status,
                    ProjectToolRequestId = rqCv.Request.ProjectToolRequestId
                });
            return query;
        }
        public IQueryable<long> IQGetInterviewHaveAnyInterviewerId(List<long> interviewIds)
        {
            return from i in WorkScope.GetAll<RequestCVInterview>()
                   where interviewIds.Contains(i.InterviewId)
                   select i.RequestCVId;
        }
    }
}
