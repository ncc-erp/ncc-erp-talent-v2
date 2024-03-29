using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.DomainServices.Requisitions.Dtos;
using TalentV2.Entities;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Requisitions
{
    public class RequisitionManager : BaseManager, IRequisitionManager
    {
        public RequisitionManager()
        {
        }

        public IQueryable<RequisitionDto> IQGetAllRequisition()
        {
            var query = from r in WorkScope.GetAll<Request>()
                        join b in WorkScope.GetAll<Branch>() on r.BranchId equals b.Id
                        join p in WorkScope.GetAll<SubPosition>() on r.SubPositionId equals p.Id
                        select new RequisitionDto
                        {
                            BranchId = r.BranchId,
                            BranchName = b.Name,
                            CreatorUserId = r.CreatorUserId,
                            CreationTime = r.CreationTime,
                            CreatorName = r.CreatorUserId.HasValue ?
                                    r.CreatorUser.FullName :
                                    "",
                            LastModifiedTime = r.LastModificationTime,
                            LastModifiedName = r.LastModifierUserId.HasValue ?
                                        r.LastModifierUser.FullName :
                                        "",
                            Id = r.Id,
                            Level = r.Level,
                            SubPositionId = r.SubPositionId,
                            SubPositionName = p.Name,
                            Priority = r.Priority,
                            Skills = r.RequestSkills.Select(s => new SkillDto
                            {
                                Id = s.SkillId,
                                Name = s.Skill.Name
                            }).ToList(),
                            Quantity = r.Quantity,
                            Status = r.Status,
                            TimeClose = r.Status == StatusRequest.Closed ? r.LastModificationTime : null,
                            QuantityOnboard = r.RequestCVs.Where(s => s.Status == RequestCVStatus.Onboarded).Count(),
                            QuantityFail = r.RequestCVs.Where(s => s.Status == RequestCVStatus.FailedInterview ||
                                                                    s.Status == RequestCVStatus.FailedTest ||
                                                                    s.Status == RequestCVStatus.RejectedInterview ||
                                                                    s.Status == RequestCVStatus.RejectedOffer)
                                                        .Count(),
                            TotalCandidateApply = r.RequestCVs.Count(),
                            QuantityAcceptedOffer = r.RequestCVs.Where(s => s.Status == RequestCVStatus.AcceptedOffer).Count(),
                            UserType = r.UserType,
                            Note = r.Note,
                            TimeNeed = r.TimeNeed,
                            ProjectToolRequestId = r.ProjectToolRequestId
                        };
            return query;
        }

        public async Task<long> CreateRequisitonStaff(CreateRequisitionStaffDto input, long resourceRequestId = default)
        {
            var requisitionStaff = ObjectMapper.Map<Request>(input);

            return await CreateRequisition(requisitionStaff, input.SkillIds, resourceRequestId);
        }

        public async Task<long> CreateRequisitionIntern(CreateRequisitionInternDto input, long resourceRequestId = default)
        {
            var requisitionIntern = ObjectMapper.Map<Request>(input);
            requisitionIntern.Id = 0;
            return await CreateRequisition(requisitionIntern, input.SkillIds, resourceRequestId);
        }

        private async Task<long> CreateRequisition(Request request, List<long> skillIds, long resourceRequestId)
        {
            request.Status = StatusRequest.InProgress;
            request.LastModificationTime = DateTimeUtils.GetNow();
            request.LastModifierUserId = AbpSession.UserId;
            if (resourceRequestId != default)
                request.ProjectToolRequestId = resourceRequestId;
            await WorkScope.InsertAndGetIdAsync(request);
            await CurrentUnitOfWork.SaveChangesAsync();
            await CreateRequestSkills(request.Id, skillIds);
            return request.Id;
        }

        public async Task<RequisitionDto> GetRequisitionById(long id)
        {
            return await IQGetAllRequisition().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<long> UpdateRequisitionStaff(UpdateRequisitionStaffDto input)
        {
            var requisition = await WorkScope.GetAsync<Request>(input.Id);
            ObjectMapper.Map(input, requisition);
            return await UpdateRequisition(requisition, input.SkillIds);
        }

        public async Task<long> UpdateRequisitionIntern(UpdateRequisitionInternDto input)
        {
            var requisition = await WorkScope.GetAsync<Request>(input.Id);
            ObjectMapper.Map(input, requisition);
            return await UpdateRequisition(requisition, input.SkillIds);
        }

        private async Task<long> UpdateRequisition(Request request, List<long> skillIds)
        {
            await DeleteManyRequestSkillByRequestId(request.Id);
            await WorkScope.UpdateAsync(request);
            await CurrentUnitOfWork.SaveChangesAsync();
            await CreateRequestSkills(request.Id, skillIds);
            return request.Id;
        }

        private async Task CreateRequestSkills(long requestId, List<long> skillIds)
        {
            foreach (var skillId in skillIds)
            {
                var requestSkill = new RequestSkill
                {
                    RequestId = requestId,
                    SkillId = skillId
                };
                await WorkScope.InsertAsync(requestSkill);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteManyRequestSkillByRequestId(long requestId)
        {
            (
                await WorkScope.GetAll<RequestSkill>()
                .Where(x => x.RequestId == requestId)
                .ToListAsync()
            )
            .ForEach(x => x.IsDeleted = true);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task CloseRequestByRequestId(long requestId, bool isProjectTool = false)
        {
            var request = await this.GetCVsByRequestId(requestId);
            var isCanClose = !request.Any(x => !CommonUtils.ListEndRequestCVStatus.Contains(x.RequestCVStatus) && x.CvStatus != CVStatus.Failed);

            var requisition = await WorkScope.GetAsync<Request>(requestId);


            if (isProjectTool)
            {
                requisition.Note = requisition.Note + ". Cancel by Project Tool";

                if (!isCanClose)
                    requisition.Quantity = 0;
                else
                    requisition.Status = StatusRequest.Closed;

                await CurrentUnitOfWork.SaveChangesAsync();
                return;
            }

            if (!isCanClose)
            {
                var endStatuses = CommonUtils.ListEndRequestCVStatus.Select(s => s.ToString()).ToArray();
                string res = string.Join(",", endStatuses);
                throw new UserFriendlyException($"Can't closed request because it has already existed status different: {res}");
            }

            requisition.Status = StatusRequest.Closed;
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task ReOpenRequestByRequestId(long requestId)
        {
            var requisition = await WorkScope.GetAsync<Request>(requestId);
            requisition.Status = StatusRequest.InProgress;
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            await WorkScope.DeleteAsync<Request>(id);
        }

        public async Task<List<CVRequisitionDto>> GetCVsByRequestId(long requestId)
        {
            var allRequestCVs = await IQGetRequestCVs()
                .Where(s => s.RequestId == requestId)
                .ToListAsync();
            return allRequestCVs;
        }

        public IQueryable<CVRequisitionDto> IQGetRequestCVs()
        {
            var now = DateTimeUtils.GetNow();
            var processStatusIds = CommonUtils.ListStatusDeadline.Select(s => (RequestCVStatus)s.Id).ToList();
            var query = from rqCv in WorkScope.GetAll<RequestCV>()
                        select new CVRequisitionDto
                        {
                            Id = rqCv.Id,
                            RequestId = rqCv.RequestId,
                            CVId = rqCv.CVId,
                            FullName = rqCv.CV.Name,
                            Email = rqCv.CV.Email,
                            Phone = rqCv.CV.Phone,
                            PathAvatar = rqCv.CV.Avatar,
                            PathLinkCV = rqCv.CV.LinkCV,
                            UserType = rqCv.CV.UserType,
                            CvStatus = rqCv.CV.CVStatus,
                            BranchId = rqCv.CV.BranchId,
                            BranchName = rqCv.CV.Branch.Name,
                            SubPositionId = rqCv.CV.SubPositionId,
                            SubPositionName = rqCv.CV.SubPosition.Name,
                            IsFemale = rqCv.CV.IsFemale,
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
                            ProcessCVStatus = (processStatusIds.Contains(rqCv.Status) && rqCv.EmailSent != true) ?
                            (
                            (rqCv.LastModificationTime.HasValue ? (now - rqCv.LastModificationTime.Value).TotalDays : (now - rqCv.CreationTime).TotalDays) <= 1 ? ProcessCVStatus.UnprocessedSendMail : ProcessCVStatus.OverdueSendMail
                            )
                            : ProcessCVStatus.None,
                        };
            return query;
        }

        public async Task<long> CloneRequestByRequestId(long requestId, int quantity = default)
        {
            var oldRequest = await WorkScope.GetAll<Request>()
                .Where(q => q.Id == requestId)
                .FirstOrDefaultAsync();
            var requestSkills = await WorkScope.GetAll<RequestSkill>()
                .Where(q => q.RequestId == requestId)
                .Select(q => q.SkillId)
                .ToListAsync();

            if (quantity != default)
            {
                oldRequest.Quantity = quantity;
                oldRequest.Note = $"Clone từ Request: {oldRequest.Id}";
            }
            oldRequest.Id = 0;

            if (!requestSkills.IsEmpty())
            {
                var newRequestSkills = new List<RequestSkill>();
                requestSkills.ForEach(skillId =>
                {
                    newRequestSkills.Add(new RequestSkill
                    {
                        Request = oldRequest,
                        SkillId = skillId
                    });
                });
                oldRequest.RequestSkills = newRequestSkills;
            }
            var newRequest = await WorkScope.InsertAsync(oldRequest);
            await CurrentUnitOfWork.SaveChangesAsync();
            return newRequest.Id;
        }

        public IQueryable<CVAvailableCloneDto> IQGetCVAvailableClone()
        {
            var query = from rqCv in WorkScope.GetAll<RequestCV>()
                        where rqCv.Status != RequestCVStatus.FailedInterview &&
                                rqCv.Status != RequestCVStatus.FailedTest &&
                                rqCv.Status != RequestCVStatus.Onboarded
                        select new CVAvailableCloneDto
                        {
                            CVId = rqCv.CVId,
                            RequestCVId = rqCv.Id,
                            RequestId = rqCv.RequestId,
                        };
            return query;
        }

        public async Task DeleteRequestCV(long requestCvId)
        {
            await DeleteCapabilityResultByRequestCVId(requestCvId);
            await DeleteInterviewByRequestCVId(requestCvId);
            await DeleteRequestCVStatusHistoryByRequestCVId(requestCvId);
            await WorkScope.DeleteAsync<RequestCV>(requestCvId);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRequestCVStatusHistoryByRequestCVId(long requestCvId)
        {
            var statusesHistory = await WorkScope.GetAll<RequestCVStatusHistory>()
                .Where(s => s.RequestCVId == requestCvId)
                .ToListAsync();
            foreach (var statusHistory in statusesHistory)
            {
                statusHistory.IsDeleted = true;
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCapabilityResultByRequestCVId(long requestCvId)
        {
            var capabilityResults = await WorkScope.GetAll<RequestCVCapabilityResult>()
                .Where(s => s.RequestCVId == requestCvId)
                .ToListAsync();
            foreach (var capabilityResult in capabilityResults)
            {
                capabilityResult.IsDeleted = true;
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteInterviewByRequestCVId(long requestCvId)
        {
            var interviews = await WorkScope.GetAll<RequestCVInterview>()
                .Where(s => s.RequestCVId == requestCvId)
                .ToListAsync();
            foreach (var interview in interviews)
            {
                interview.IsDeleted = true;
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<List<long>> GetRequestIdsHaveAllSkillAsync(List<long> skillIds)
        {
            if (skillIds == null || skillIds.IsEmpty())
            {
                throw new Exception("skillIds null or empty");
            }

            var result = await WorkScope.GetAll<RequestSkill>()
                    .Where(s => skillIds[0] == s.SkillId)
                    .Select(s => s.RequestId)
                    .Distinct()
                    .ToListAsync();

            if (result == null || result.IsEmpty())
            {
                return new List<long>();
            }
            var count = skillIds.Count();
            for (var i = 1; i < count; i++)
            {
                var requestIds = await WorkScope.GetAll<RequestSkill>()
                    .Where(s => skillIds[i] == s.SkillId)
                    .Select(s => s.RequestId)
                    .Distinct()
                    .ToListAsync();

                result = result.Intersect(requestIds).ToList();

                if (result == null || result.IsEmpty())
                {
                    return new List<long>();
                }
            }

            return result;
        }

        public IQueryable<long> IQGetRequestHaveAnySkill(List<long> skillIds)
        {
            if (skillIds == null || skillIds.IsEmpty())
            {
                throw new Exception("skillIds null or empty");
            }
            return WorkScope.GetAll<RequestSkill>()
                   .Where(s => skillIds.Contains(s.SkillId))
                   .Select(s => s.RequestId);
        }

        public async Task<List<long>> GetCVIdsByRequestId(long requestId)
        {
            return await WorkScope.GetAll<RequestCV>()
                .Where(q => q.RequestId == requestId)
                .Select(s => s.CVId)
                .ToListAsync();
        }

        public async Task<RequisitionToCloseAndCloneDto> GetRequisitionToCloseAndClone(long requestId)
        {
            var query = from r in WorkScope.GetAll<Request>()
                        where r.Id == requestId
                        select new RequisitionToCloseAndCloneDto
                        {
                            Id = r.Id,
                            BranchId = r.BranchId,
                            Note = r.Note,
                            SubPositionId = r.SubPositionId,
                            Priority = r.Priority,
                            Quantity = r.Quantity,
                            SkillIds = r.RequestSkills.Select(s => s.SkillId).ToList(),
                            UserType = r.UserType,
                            CandidateRequisitions = r.RequestCVs
                            .Select(s => new CandidateRequisitionDto
                            {
                                Avatar = CommonUtils.FullFilePath(s.CV.Avatar),
                                Email = s.CV.Email,
                                FullName = s.CV.Name,
                                IsFemale = s.CV.IsFemale,
                                CVStatus = s.CV.CVStatus,
                                CvStatusName = Enum.GetName(s.CV.CVStatus),
                                LinkCV = CommonUtils.FullFilePath(s.CV.LinkCV),
                                Id = s.CV.Id,
                                Phone = s.CV.Phone,
                                Status = s.Status,
                            }).ToList()
                        };
            return await query.FirstOrDefaultAsync();
        }

        public IQueryable<RequisitionToCloseAndCloneAllDto> IQGetAllCloseAndCloneRequisition()
        {
            var query = from r in WorkScope.GetAll<Request>()
                        where r.Status == StatusRequest.InProgress
                        where r.UserType == UserType.Intern
                        join b in WorkScope.GetAll<Branch>() on r.BranchId equals b.Id
                        join p in WorkScope.GetAll<SubPosition>() on r.SubPositionId equals p.Id
                        select new RequisitionToCloseAndCloneAllDto
                        {
                            BranchName = b.Name,
                            Id = r.Id,
                            SubPositionName = p.Name,
                            Priority = r.Priority,
                            Quantity = r.Quantity,
                            Note = r.Note,
                            TimeNeed = r.TimeNeed,
                            Skills = r.RequestSkills.Select(s => new SkillDto
                            {
                                Id = s.SkillId,
                                Name = s.Skill.Name
                            }).ToList(),
                            LastModifiedTime = r.LastModificationTime
                        };
            return query;
        }

        public async Task<CloneRequisitionDto> GetRequisitionByRequestId(long requestId, DateTime? timeNeed, string note, int quantity)
        {
            var query = from r in WorkScope.GetAll<Request>()
                        where r.Id == requestId
                        join b in WorkScope.GetAll<Branch>() on r.BranchId equals b.Id
                        join p in WorkScope.GetAll<SubPosition>() on r.SubPositionId equals p.Id
                        select new CloneRequisitionDto
                        {
                            UserType = r.UserType,
                            SubPositionId = r.SubPositionId,
                            Priority = r.Priority,
                            SkillIds = r.RequestSkills.Select(s => s.SkillId).ToList(),
                            BranchId = r.BranchId,
                            Quantity = quantity,
                            Note = note,
                            TimeNeed = timeNeed.Value,
                            Id = requestId,
                            CVIds = r.RequestCVs.Where(s => !CommonUtils.ListEndRequestCVStatus.Contains(s.Status)).Select(x => x.CV.Id).ToList()
                        };
            return await query.FirstOrDefaultAsync();
        }
    }
}