using Abp.Authorization;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc;
using NccCore.Extension;
using NccCore.Paging;
using System.Threading.Tasks;
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
        [AbpAllowAnonymous]
        public async Task<GridResult<BackgroundWorkerDto>> GetAllPaging(GridParam param)
        {
            var query = _backgroundWorkerManager.IQGetAllBackgroundWorker();
            return await query.GetGridResult(query, param);
        }

        [HttpPut]
        [AbpAllowAnonymous]
        public BackgroundWorkerDto Update(BackgroundWorkerDto workerInput)
        {
            return _backgroundWorkerManager.UpdateActivedWorker(workerInput);
        }
    }
}
