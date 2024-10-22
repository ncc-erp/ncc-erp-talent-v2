using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using NccCore.Extension;
using NccCore.Paging;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.DomainServices.Webhook;
using TalentV2.DomainServices.Webhook.Dtos;
using TalentV2.WebServices.ExternalServices.MezonWebhooks;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class MezonWebhookAppService : TalentV2AppServiceBase
    {
        private readonly IMezonWebhookManager _mezonWebhookManager;
        protected readonly MezonWebhookService _mezonWebhookService;
        public MezonWebhookAppService(
            IMezonWebhookManager mezonWebhookManager, 
            MezonWebhookService mezonWebhookService)
        {
            _mezonWebhookManager = mezonWebhookManager;
            _mezonWebhookService = mezonWebhookService;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_MezonWebhooks_ViewList)]
        public async Task<GridResult<MezonWebhookDto>> GetAllPaging(GridParam param)
        {
            var query = _mezonWebhookManager.IQGetAllMezonWebhook();
            return await query.GetGridResult(query, param);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_MezonWebhooks_ViewList)]
        public async Task<MezonWebhookDto> Get(long id)
        {
            return await _mezonWebhookManager.GetMezonWebhook(id);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_MezonWebhooks_Edit)]
        public async Task<MezonWebhookDto> Update(MezonWebhookDto input)
        {
            return await _mezonWebhookManager.UpdateMezonWebhook(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_MezonWebhooks_Create)]
        public async Task<MezonWebhookDto> Create(MezonWebhookDto input)
        {
            return await _mezonWebhookManager.CreateMezonWebhook(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_MezonWebhooks_Delete)]
        public async Task<string> Delete(long Id)
        {
            await _mezonWebhookManager.DeleteMezonWebhook(Id);
            return "Deleted Successfully!";
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_MezonWebhooks_ViewList)]
        public async Task Notify()
        {
            await _mezonWebhookService.NotifyToWebhookUrl("NotifyToWebhookUrl", "https://webhook.mezon.ai/webhooks/1833361163941842944/MTcyODQ0OTU3MjE3MzQ1NTcyMjoxNzc5NDg0NTA0Mzc3NzkwNDY0OjE4MzMzNjExNjM5NDE4NDI5NDQ.aBOrllerpGbwoQtzRsFLuBd1PGGuP6cHtiN1i3gma8w");
        }
    }
}
