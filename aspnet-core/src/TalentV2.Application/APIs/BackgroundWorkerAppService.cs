using Abp.Authorization;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc;
using NccCore.Extension;
using NccCore.Paging;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.DomainServices.BackgroundWorkers.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class BackgroundWorkerAppService: TalentV2AppServiceBase
    {
        private readonly DomainServices.BackgroundWorkers.IBackgroundWorkerManager _backgroundWorkerManager;

        public BackgroundWorkerAppService()
        {
            _backgroundWorkerManager = IocManager.Instance.Resolve<DomainServices.BackgroundWorkers.IBackgroundWorkerManager>();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_BackgroundWorkers_ViewList)]
        public async Task<GridResult<BackgroundWorkerDto>> GetAllPaging(GridParam param)
        {
            var query = _backgroundWorkerManager.IQGetAllBackgroundWorker();
            return await query.GetGridResult(query, param);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_BackgroundWorkers_Update)]
        public BackgroundWorkerDto Update(BackgroundWorkerDto workerInput)
        {
            return _backgroundWorkerManager.UpdateActivedWorker(workerInput);
        }
    }
}
