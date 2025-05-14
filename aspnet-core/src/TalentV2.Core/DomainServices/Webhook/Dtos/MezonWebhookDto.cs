using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NccCore.Anotations;
using System.Collections.Generic;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Webhook.Dtos
{

    [AutoMap(typeof(MezonWebhook))]
    public class MezonWebhookDto : EntityDto<long>
    {
        [ApplySearch]
        public string Name { get; set; }
        public string Url { get; set; }
        public List<string> Functions { get; set; }
        public bool IsActive { get; set; }
    }
}
