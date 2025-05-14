using Abp.Domain.Services;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.DomainServices.Webhook.Dtos;

namespace TalentV2.DomainServices.Webhook
{
    public interface IMezonWebhookManager : IDomainService
    {
        IQueryable<MezonWebhookDto> IQGetAllMezonWebhook();
        Task<MezonWebhookDto> GetMezonWebhook(long id);
        Task<MezonWebhookDto> CreateMezonWebhook(MezonWebhookDto input);
        Task<MezonWebhookDto> UpdateMezonWebhook(MezonWebhookDto input);
        Task DeleteMezonWebhook(long Id);
    }
}
