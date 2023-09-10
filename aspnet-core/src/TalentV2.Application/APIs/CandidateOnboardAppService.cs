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
using TalentV2.WebServices.InternalServices.HRM;
using TalentV2.WebServices.InternalServices.HRM.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class CandidateOnboardAppService : TalentV2AppServiceBase
    {
        private readonly IRequestCVManager _candidateOnboardManager;
        private readonly ICandidateManager _candidateManager;
        private readonly HRMService _hrmService;
        public CandidateOnboardAppService(
            IRequestCVManager candidateOnboardManager,
            ICandidateManager candidateManager,
            HRMService hrmService
        )
        {
            _candidateOnboardManager = candidateOnboardManager;
            _candidateManager = candidateManager;
            _hrmService = hrmService;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Onboards_ViewList)]
        public async Task<GridResult<CandidateOfferDto>> GetAllPaging(CandidateOnboardFilterPaging param)
        {
            var query = _candidateOnboardManager
                .IQGetRequestCV()
                .Where(q => CommonUtils.ListStatusCandidateOnboard.Select(x => (RequestCVStatus)x.Id).Contains(q.RequestCVStatus))
                .Where(q => !param.FromDate.HasValue || q.OnboardDate.Value.Date >= param.FromDate.Value.Date)
                .Where(q => !param.ToDate.HasValue || q.OnboardDate.Value.Date <= param.ToDate.Value.Date) ;

            return await query.GetGridResult(query, param);
            
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_Onboards_Edit)]
        public async Task<CandidateOfferDto> Update(UpdateCandidateOnboardDto input)
        {
            var requestCV = await WorkScope.GetAsync<RequestCV>(input.Id);
            var requestCvId = await _candidateOnboardManager.UpdateCandidateOnboard(input);
            await _candidateManager.CreateRequestCVHistory(new HistoryRequestCVDto
            {
                Id = requestCvId,
                Status = input.Status,
                OnboardDate = input.OnboardDate,
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

            return await _candidateOnboardManager
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
