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
using TalentV2.DomainServices.RequestCVs;
using TalentV2.DomainServices.RequestCVs.Dtos;
using TalentV2.Entities;
using TalentV2.Utils;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class CandidateOfferAppService : TalentV2AppServiceBase
    {
        private readonly IRequestCVManager _candidateOfferManager;
        private readonly ICandidateManager _candidateManager;
        public CandidateOfferAppService(IRequestCVManager candidateOfferManager, ICandidateManager candidateManager)
        {
            _candidateOfferManager = candidateOfferManager; 
            _candidateManager = candidateManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Offers_ViewList)]
        public async Task<GridResult<CandidateOfferDto>> GetAllPaging(GridParam param)
        {
            var query = _candidateOfferManager
                .IQGetRequestCV()
                .Where(q => CommonUtils.ListStatusCandidateOffer.Select(x => (RequestCVStatus)x.Id).Contains(q.RequestCVStatus));

            var filterStatus = param.FilterItems.FirstOrDefault(s => s.PropertyName == "requestCVStatus");
            if (filterStatus == null)
                return await query.GetGridResult(query, param);

            var status = filterStatus.Value as RequestCVStatus?;
            if (status != RequestCVStatus.RejectedOffer)
            {
                query = query.Where(s => s.RequestStatus == StatusRequest.InProgress);
                return await query.GetGridResult(query, param);
            }
            return await query.GetGridResult(query, param);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_Offers_Edit)]
        public async Task<CandidateOfferDto> Update(UpdateCandidateOfferDto input)
        {
            var requestCV = await WorkScope.GetAsync<RequestCV>(input.Id);
            var requestCvId = await _candidateOfferManager.UpdateCandidateOffer(input);
            await _candidateManager.CreateRequestCVHistory(new HistoryRequestCVDto
            {
                Id = requestCvId,
                Status = input.Status,
            });
            await _candidateManager.CreateRequestCVStatusChangeHistory(new StatusChangeRequestCVDto
            {
                Id = requestCvId,
                FromStatus = requestCV.Status,
                ToStatus = input.Status,
            });

            if (input.Status == RequestCVStatus.AcceptedOffer || input.Status == RequestCVStatus.RejectedOffer)
            {
                _candidateManager.UpdateHrmTempEmployee(requestCvId);
            }

            return await _candidateOfferManager
                .IQGetRequestCV()
                .FirstOrDefaultAsync(s => s.Id == requestCvId);
        }

        [HttpGet]
        public async Task<long> UpdateHrmTempEmployee(long requestCvId)
        {
            _candidateManager.UpdateHrmTempEmployee(requestCvId);

            return requestCvId;
        }
    }
}
