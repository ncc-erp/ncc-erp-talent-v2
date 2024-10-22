using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.DomainServices.Webhook.Dtos;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Webhook
{
    public class MezonWebhookManager : BaseManager, IMezonWebhookManager
    {

        public IQueryable<MezonWebhookDto> IQGetAllMezonWebhook()
        {
            var qallMezonWebhook = from m in WorkScope.GetAll<MezonWebhook>()
                                   select new MezonWebhookDto
                                   {
                                       Id = m.Id,
                                       Name = m.Name,
                                       Url = m.Url,
                                       IsActive = m.IsActive,
                                       Destination = m.Destination,
                                   };
            return qallMezonWebhook;
        }

        public async Task<MezonWebhookDto> GetMezonWebhook(long id)
        {
            MezonWebhook webhook = await WorkScope.GetAsync<MezonWebhook>(id);
            return ObjectMapper.Map<MezonWebhookDto>(webhook);
        }

        public async Task<MezonWebhookDto> UpdateMezonWebhook(MezonWebhookDto input)
        {
            MezonWebhook webhook = await WorkScope.GetAsync<MezonWebhook>(input.Id);
            if (webhook == null)
            {
                throw new UserFriendlyException("Mezon Webhook have not already existed!");
            }

            CheckUrlAndDestinationMaxLength(input);

            webhook.Name = input.Name.Trim();
            webhook.Url = input.Url.Trim();
            webhook.IsActive = input.IsActive;
            webhook.Destination = input.Destination.Trim();

            await WorkScope.UpdateAsync(webhook);
            return IQGetAllMezonWebhook().FirstOrDefault(s => s.Id == webhook.Id);
        }

        public async Task<MezonWebhookDto> CreateMezonWebhook(MezonWebhookDto input)
        {
            CheckUrlAndDestinationMaxLength(input);

            input.Name = input.Name.Trim();
            input.Url = input.Url.Trim();
            input.Destination = input.Destination.Trim();

            MezonWebhook webhook = ObjectMapper.Map<MezonWebhook>(input);

            long id = await WorkScope.InsertOrUpdateAndGetIdAsync<MezonWebhook>(webhook);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllMezonWebhook()
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteMezonWebhook(long Id)
        {
            MezonWebhook webhook = await WorkScope.GetAsync<MezonWebhook>(Id);
            webhook.IsDeleted = true;
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private void CheckUrlAndDestinationMaxLength(MezonWebhookDto input)
        {
            var maxUrlLength = typeof(MezonWebhook)
                .GetProperty("Url")
                .GetCustomAttributes(typeof(MaxLengthAttribute), false)
                .Cast<MaxLengthAttribute>()
                .FirstOrDefault()?.Length;
            if (input.Url.Length > maxUrlLength)
            {
                throw new UserFriendlyException($"Webhook Url length is greater than {maxUrlLength}!");
            }

            var maxDestinationLength = typeof(MezonWebhook)
                .GetProperty("Url")
                .GetCustomAttributes(typeof(MaxLengthAttribute), false)
                .Cast<MaxLengthAttribute>()
                .FirstOrDefault()?.Length;
            if (input.Url.Length > maxDestinationLength)
            {
                throw new UserFriendlyException($"Webhook Destination length is greater than {maxDestinationLength}!");
            }
        }
    }
}
