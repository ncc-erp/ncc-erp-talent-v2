using Abp.Authorization;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Requisitions;
using TalentV2.DomainServices.Requisitions.Dtos;
using TalentV2.Entities;
using TalentV2.Utils;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class RequisitionInternAppService : TalentV2AppServiceBase
    {
        private readonly IRequisitionManager _requisitionManager;

        private readonly ICandidateManager _candidateManager;

        public RequisitionInternAppService(IRequisitionManager requisitionManager, ICandidateManager candidateManager)
        {
            _requisitionManager = requisitionManager;
            _candidateManager = candidateManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_ViewList)]
        public async Task<ResultRequisition<RequisitionDto>> GetAllPaging(RequisitionFilterPagingDto param)
        {
            var query = _requisitionManager
                .IQGetAllRequisition()
                .Where(q => q.UserType == UserType.Intern);

            if (param.SkillIds == null || param.SkillIds.IsEmpty())
            {
            }
            else if (param.SkillIds.Count() == 1 || !param.IsAndCondition)
            {
                var qrequestIdsHaveAnySkill = _requisitionManager
                    .IQGetRequestHaveAnySkill(param.SkillIds)
                    .Distinct();
                query = from r in query
                        join requestId in qrequestIdsHaveAnySkill on r.Id equals requestId
                        select r;
            }
            else
            {
                var requestIds = await _requisitionManager.GetRequestIdsHaveAllSkillAsync(param.SkillIds);
                query = query.Where(x => requestIds.Contains(x.Id));
            }
            var result = await query.GetGridResult(query, param);
            return new ResultRequisition<RequisitionDto>
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalQuantity = await query.ApplySearchAndFilter(param).SumAsync(s => s.Quantity),
            };
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_Create)]
        public async Task<RequisitionDto> Create(CreateRequisitionInternDto input)
        {
            var requestId = await _requisitionManager.CreateRequisitionIntern(input);
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(x => x.Id == requestId);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_AddCV)]
        public async Task<object> CreateRequestCV(long requestId, long cvId,long presentrequestId)
        {
            var result = await _candidateManager.CreateRequestCV(new CandidateRequestDto
            {
                CvId = cvId,
                RequestId = requestId,
                PresentrequestId = presentrequestId
            });
            var cv = await _candidateManager
                .IQGetAllCVs()
                .FirstOrDefaultAsync(s => s.Id == cvId);

            var requisition = await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(s => s.Id == requestId);
            var response = new
            {
                CV = cv,
                Requisition = requisition
            };
            return response;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_AddCV)]
        public async Task<IActionResult> CreateManyRequestCV(AddCVDto input)
        {
            var resultCreateMany = new CreateRequestCVResultDto();
            foreach (var cvId in input.CVIds)
            {
                var cvIdSuccess = await _candidateManager.CreateRequestCV(new CandidateRequestDto
                {
                    CvId = cvId,
                    RequestId = input.RequestId,
                });
                if (cvIdSuccess == default)
                    resultCreateMany.CVIdsFail.Add(cvId);
                else
                    resultCreateMany.CVIdsSuccess.Add(cvId);
            }
            var requisition = await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(s => s.Id == input.RequestId);
            var response = new
            {
                RequestCV = resultCreateMany,
                Requisition = requisition
            };
            return new OkObjectResult(response);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_Clone)]
        public async Task<RequisitionDto> CloneRequest(long requestId)
        {
            var newRequestId = await _requisitionManager.CloneRequestByRequestId(requestId);
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(q => q.Id == newRequestId);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_CloseAndClone)]
        public async Task<RequisitionToCloseAndCloneDto> GetRequisitionToCloseAndClone(long requestId)
        {
            return await _requisitionManager.GetRequisitionToCloseAndClone(requestId);
        }

        [HttpPost]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_CloseAndClone)]
        public async Task<RequisitionDto> CloseAndCloneRequest(CloneRequisitionDto input)
        {
            var oldRequisition = await WorkScope.GetAsync<Request>(input.Id);
            oldRequisition.Status = StatusRequest.Closed;
            await CurrentUnitOfWork.SaveChangesAsync();

            var newRequisitionId = await _requisitionManager.CreateRequisitionIntern(input);
            var cvIds = input.CVIds;
            var new_cvids = WorkScope.GetAll<CV>()
                .Where(c => cvIds.Contains(c.Id))
                .Where(c => c.CVStatus != CVStatus.Failed)
                .Where(c => c.RequestCVs.Where(x =>
                    x.Status != RequestCVStatus.RejectedOffer ||
                    x.Status != RequestCVStatus.RejectedInterview ||
                    x.Status != RequestCVStatus.FailedTest ||
                    x.Status != RequestCVStatus.RejectedApply ||
                    x.Status != RequestCVStatus.RejectedTest ||
                    x.Status != RequestCVStatus.FailedInterview ||
                    x.Status != RequestCVStatus.Onboarded).Any())
                .Select(c => c.Id).ToList();

            await CreateManyRequestCVAsync(new AddCVDto
            {
                CVIds = new_cvids,
                RequestId = newRequisitionId,
                OldRequestId = input.Id,
                UserType = UserType.Intern,
                SubPositionId = input.SubPositionId,
            });
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(s => s.Id == newRequisitionId);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_Edit)]
        public async Task<RequisitionDto> Update(UpdateRequisitionInternDto input)
        {
            var requestId = await _requisitionManager.UpdateRequisitionIntern(input);
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(q => q.Id == requestId);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            await _requisitionManager.Delete(id);
            return new OkObjectResult("Deleted Successfully");
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_ViewDetail)]
        public async Task<RequisitionDto> GetById(long id)
        {
            return await _requisitionManager.GetRequisitionById(id);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_ViewDetail)]
        public async Task<List<CVRequisitionDto>> GetCVsByRequestId(long requestId)
        {
            return await _requisitionManager.GetCVsByRequestId(requestId);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_DeleteRequestCV)]
        public async Task<object> DeleteRequestCV(long requestCVId, long requestId)
        {
            await _requisitionManager.DeleteRequestCV(requestCVId);
            var requisition = await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(s => s.Id == requestId);
            return requisition;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_Close)]
        public async Task<RequisitionDto> CloseRequest(long requestId)
        {
            await _requisitionManager.CloseRequestByRequestId(requestId);
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(q => q.Id == requestId);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionIntern_ReOpen)]
        public async Task<RequisitionDto> ReOpenRequest(long requestId)
        {
            await _requisitionManager.ReOpenRequestByRequestId(requestId);
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(q => q.Id == requestId);
        }

        [HttpGet]
        public async Task<List<long>> GetCVIdsByReqquestId(long requestId)
        {
            return await _requisitionManager.GetCVIdsByRequestId(requestId);
        }

        private async Task<CreateRequestCVResultDto> CreateManyRequestCVAsync(AddCVDto input)
        {
            var oldRequestCVs = await WorkScope.GetAll<RequestCV>()
                    .Where(q => q.RequestId == input.OldRequestId)
                    .Select(s => new
                    {
                        s.CVId,
                        s.Status,
                        s.InterviewTime,
                        s.ApplyLevel,
                        s.FinalLevel,
                        s.InterviewLevel,
                        s.HRNote,
                        s.Salary,
                        s.OnboardDate,
                        s.LMSInfo,
                        s.EmailSent,
                        InterviewIds = s.RequestCVInterviews
                                        .Where(x => !x.IsDeleted)
                                        .Select(x => x.InterviewId)
                                        .ToList(),
                        Capabilities = s.RequestCVCapabilityResults
                                        .Where(x => !x.IsDeleted)
                                        .Select(x => new { x.CapabilityId, x.Score, x.Note, x.Factor })
                                        .ToList(),
                    })
                    .ToDictionaryAsync(x => x.CVId);

            CreateRequestCVResultDto response = new();
            List<RequestCV> requestCVs = new();

            var cvIds = oldRequestCVs.Keys.Intersect(input.CVIds).ToList();

            foreach (var cvId in cvIds)
            {
                var requestCv = new RequestCV
                {
                    RequestId = input.RequestId,
                    CVId = cvId,
                    Status = oldRequestCVs[cvId].Status,
                    InterviewTime = oldRequestCVs[cvId].InterviewTime,
                    ApplyLevel = oldRequestCVs[cvId].ApplyLevel,
                    FinalLevel = oldRequestCVs[cvId].FinalLevel,
                    LMSInfo = oldRequestCVs[cvId].LMSInfo,
                    OnboardDate = oldRequestCVs[cvId].OnboardDate,
                    HRNote = oldRequestCVs[cvId].HRNote,
                    Salary = oldRequestCVs[cvId].Salary,
                    EmailSent = oldRequestCVs[cvId].EmailSent
                };

                List<RequestCVCapabilityResult> requestCVCapabilities = new();
                if (oldRequestCVs[cvId].Capabilities != null)
                {
                    oldRequestCVs[cvId].Capabilities.ForEach(capability =>
                    {
                        requestCVCapabilities.Add(new RequestCVCapabilityResult
                        {
                            CapabilityId = capability.CapabilityId,
                            RequestCV = requestCv,
                            Score = capability.Score,
                            Note = capability.Note,
                            Factor = capability.Factor
                        });
                    });
                }
                requestCv.RequestCVCapabilityResults = requestCVCapabilities;

                List<RequestCVInterview> requestCVInterviews = new();
                if (oldRequestCVs[cvId].InterviewIds != null)
                {
                    oldRequestCVs[cvId].InterviewIds.ForEach(interviewId =>
                    {
                        requestCVInterviews.Add(new RequestCVInterview
                        {
                            RequestCV = requestCv,
                            InterviewId = interviewId,
                        });
                    });
                }
                requestCv.RequestCVInterviews = requestCVInterviews;

                requestCv.RequestCVStatusHistories = new List<RequestCVStatusHistory>
                {
                    new RequestCVStatusHistory
                    {
                        RequestCV = requestCv,
                        Status = oldRequestCVs[cvId].Status,
                        TimeAt = DateTimeUtils.GetNow(),
                    }
                };
                requestCv.RequestCVStatusChangeHistoies = new List<RequestCVStatusChangeHistory>
                {
                    new RequestCVStatusChangeHistory
                    {
                        RequestCV = requestCv,
                        FromStatus = null,
                        ToStatus = oldRequestCVs[cvId].Status,
                        TimeAt = DateTimeUtils.GetNow(),
                    }
                };
                requestCVs.Add(requestCv);
                response.CVIdsSuccess.Add(cvId);
            }

            if (requestCVs.Count > 0)
                await AddRangeAsync(requestCVs);

            return response;
        }

        [HttpGet]
        public async Task<List<RequisitionToCloseAndCloneAllDto>> GetAllCloseAndCloneRequisition()
        {
            return await _requisitionManager
               .IQGetAllCloseAndCloneRequisition()
               .ToListAsync();
        }

        [HttpPost]
        [UnitOfWork(isTransactional: false)]
        public async Task<string> CloseAndCloneAllRequisition(InputRequisitionToCloseAndCloneAllDto input)
        {
            foreach (var item in input.ListRequisitionIntern)
            {
                var inputClone = await GetRequisitionByRequestId(item.RequestId, item.TimeNeed, item.Note, item.Quantity);
                if (inputClone.Quantity > 0 || inputClone.CVIds.Count() > 0)
                {
                    await CloseAndCloneRequest(inputClone);
                }
                else
                {
                    var oldRequisition = await WorkScope.GetAsync<Request>(item.RequestId);
                    oldRequisition.Status = StatusRequest.Closed;
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            return null;
        }

        private async Task<CloneRequisitionDto> GetRequisitionByRequestId(long requestId, DateTime? timeNeed, string note, int quantity)
        {
            return await _requisitionManager.GetRequisitionByRequestId(requestId, timeNeed, note, quantity);
        }
    }
}