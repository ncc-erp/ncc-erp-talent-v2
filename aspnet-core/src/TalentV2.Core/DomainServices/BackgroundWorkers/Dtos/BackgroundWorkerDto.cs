using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.BackgroundWorkers.Dtos
{
    [AutoMap(typeof(Entities.BackgroundWorker))]
    public class BackgroundWorkerDto: EntityDto<long>
    {
        [ApplySearchAttribute]
        public string Name { get; set; }
        public int? TenantId { get; set; }
        public BackgroundWorkerState State { get; set; }
        public int Period { get; set; }
        public bool IsPaused { get; set; }
    }
}
