using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.DomainServices.ApplyCVs;
using TalentV2.DomainServices.ApplyCVs.Dtos;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Candidates;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class ApplyCVAppService : TalentV2AppServiceBase
    {
        private readonly IApplyCvManager _applyCVManager;
        private readonly ICandidateManager _candidateManager;
        public ApplyCVAppService(IApplyCvManager applyCvManager, ICandidateManager candidateManager)
        {
            _applyCVManager = applyCvManager;
            _candidateManager = candidateManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_ApplyCV_ViewList)]
        public async Task<GridResult<ApplyCVDto>> GetAllPaging(ApplyCvFilterPaging param)
        {
            var query = _applyCVManager.IQGetAll()
    
              .Where(q => !param.FromDate.HasValue || q.ApplyCVDate.Value.Date >= param.FromDate.Value.Date)
              .Where(q => !param.ToDate.HasValue || q.ApplyCVDate.Value.Date <= param.ToDate.Value.Date);

            if (param.UserType.HasValue)
            {
                var str = param.UserType.HasValue ? UserType.GetName<UserType>(param.UserType.Value) : null;
                query = query.Where(q => q.PositionType.ToLower() == str.ToLower());
            }
            
            if (param.BranchId.HasValue)
            {
                query = query.Where(q => q.BranchId == param.BranchId);
            }
            var result = await query.GetGridResult(query, param);
            return result;
        }

        [HttpPost]
        public async Task<PersonBioDto> Create([FromForm] CreateCandidateDto param)
        {
            var cvId = await _candidateManager.CreateCV(param);
            return await _candidateManager.GetCVById(cvId);
        }
        
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_ApplyCV_Create)]
        public async Task<ApplyCVDto> GetCVById(long cvId)
        {
            return await _applyCVManager.GetCVById(cvId);
        }
    }
}
