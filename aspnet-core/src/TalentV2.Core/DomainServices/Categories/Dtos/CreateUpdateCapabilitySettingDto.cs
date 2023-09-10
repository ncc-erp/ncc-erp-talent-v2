using TalentV2.Constants.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Entities;
using Abp.AutoMapper;

namespace TalentV2.DomainServices.Categories.Dtos
{
    [AutoMapTo(typeof(CapabilitySetting))]
    public class CreateUpdateCapabilitySettingDto
    {
        public long Id { get; set; }
        public UserType UserType { get; set; }
        public long SubPositionId { get; set; }
        public long CapabilityId { get; set; }
        public string Note { get; set; }
        public string GuideLine { get; set; }
        public int Factor { get; set; } = 1;
    }
}
