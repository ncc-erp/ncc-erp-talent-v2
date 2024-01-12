using Abp.Authorization;
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
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.DomainServices.Requisitions;
using TalentV2.DomainServices.Requisitions.Dtos;
using TalentV2.Entities;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class RequisitionStaffAppService : TalentV2AppServiceBase
    {
        private readonly IRequisitionManager _requisitionManager;
        private readonly ICandidateManager _candidateManager;
        public RequisitionStaffAppService
        (
            IRequisitionManager requisitionManager,
            ICandidateManager candidateManager
        )
        {
            _requisitionManager = requisitionManager;
            _candidateManager = candidateManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_ViewList)]
        public async Task<ResultRequisition<RequisitionDto>> GetAllPaging(RequisitionFilterPagingDto param)
        {
            var query = _requisitionManager
                .IQGetAllRequisition()
                .Where(q => q.UserType == UserType.Staff);

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
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_Create)]
        public async Task<RequisitionDto> Create(CreateRequisitionStaffDto input)
        {
            var requestId = await _requisitionManager.CreateRequisitonStaff(input);
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(x => x.Id == requestId);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_AddCV)]
        public async Task<object> CreateRequestCV(CandidateRequestDto input)
        {
            var result = await _candidateManager.CreateRequestCV(new CandidateRequestDto
            {
                CvId = input.CvId,
                RequestId = input.RequestId,
                CurrentRequestId = input.CurrentRequestId,
                PresenForHr = input.PresenForHr,
            });
            var cv = await _candidateManager
                .IQGetAllCVs()
                .FirstOrDefaultAsync(s => s.Id == input.CvId);

            var requisition = await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(s => s.Id == input.RequestId);
            var response = new
            {
                CV = cv,
                Requisition = requisition
            };
            return response;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_AddCV)]
        public async Task<object> CreateManyRequestCV(AddCVDto input)
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
            return response;
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_Clone)]
        public async Task<RequisitionDto> CloneRequest(long requestId)
        {
            var newRequestId = await _requisitionManager.CloneRequestByRequestId(requestId);
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(q => q.Id == newRequestId);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_Close)]
        public async Task<RequisitionDto> CloseRequest(long requestId)
        {
            await _requisitionManager.CloseRequestByRequestId(requestId);
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(q => q.Id == requestId);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_ReOpen)]
        public async Task<RequisitionDto> ReOpenRequest(long requestId)
        {
            await _requisitionManager.ReOpenRequestByRequestId(requestId);
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(q => q.Id == requestId);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_Edit)]
        public async Task<RequisitionDto> Update(UpdateRequisitionStaffDto input)
        {
            var requestId = await _requisitionManager.UpdateRequisitionStaff(input);
            return await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(q => q.Id == requestId);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            await _requisitionManager.Delete(id);
            return new OkObjectResult("Deleted Successfully");
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_ViewDetail)]
        public async Task<RequisitionDto> GetById(long id)
        {
            return await _requisitionManager.GetRequisitionById(id);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_ViewDetail)]
        public async Task<List<CVRequisitionDto>> GetCVsByRequestId(long requestId)
        {
            return await _requisitionManager.GetCVsByRequestId(requestId);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_RequisitionStaff_DeleteRequestCV)]
        public async Task<RequisitionDto> DeleteRequestCV(long requestCVId, long requestId)
        {
            await _requisitionManager.DeleteRequestCV(requestCVId);
            var requisition = await _requisitionManager
                .IQGetAllRequisition()
                .FirstOrDefaultAsync(s => s.Id == requestId);
            return requisition;
        }
        [HttpGet]
        public async Task<List<long>> GetCVIdsByReqquestId(long requestId)
        {
            return await _requisitionManager.GetCVIdsByRequestId(requestId);
        }
    }
}
