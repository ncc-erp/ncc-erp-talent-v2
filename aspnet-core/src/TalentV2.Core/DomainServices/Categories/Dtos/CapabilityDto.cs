using Abp.AutoMapper;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Categories.Dtos
{
    [AutoMapTo(typeof(Capability))]
    public class CapabilityDto : CategoryDto
    {
        [ApplySearchAttribute]
        public string Note { get; set; }
        public bool FromType { get; set; }
    }
    public class CapabilityCheckListDto
    {
        public long Id { get; set; }
        public string CapabilityName { get; set; }
        public string Note { get; set; }
    }
}
