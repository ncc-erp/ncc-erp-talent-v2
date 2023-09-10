using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.PositionSettings.Dtos
{
    [AutoMapTo(typeof(PositionSetting))]
    public class CreatePositionSettingDto
    {
        [Required]
        public UserType UserType { get; set; }
        [Required]
        public long SubPositionId { get; set; }
    }
}
