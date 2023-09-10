using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using NccCore.Extension;
using NccCore.Paging;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.DomainServices.ExternalCVs;
using TalentV2.DomainServices.ExternalCVs.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class ExternalCVAppService : TalentV2AppServiceBase
    {
        private readonly IExternalCVManager _externalCVManager;
        public ExternalCVAppService(IExternalCVManager externalCVManager)
        {
            _externalCVManager = externalCVManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_ExternalCVs_ViewList)]
        public async Task<GridResult<ExternalCVDto>> GetAllPaging(GridParam param)
        {
            var query = _externalCVManager.IQGetAllExternalCVs();
            return await query.GetGridResult(query, param);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_ExternalCVs_ViewDetail)]
        public async Task<ExternalCVDto> GetExternalCVById(long cvId)
        {
            return await _externalCVManager.GetExternalCVById(cvId);
        }
    }
}
