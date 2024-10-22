using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Entities;
using TalentV2.MultiTenancy;

namespace TalentV2.DomainServices.Webhook.Dtos
{

    [AutoMap(typeof(MezonWebhook))]
    public class MezonWebhookDto : EntityDto<long>
    {
        [ApplySearchAttribute]
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public string Destination { get; set; }

    }
}
