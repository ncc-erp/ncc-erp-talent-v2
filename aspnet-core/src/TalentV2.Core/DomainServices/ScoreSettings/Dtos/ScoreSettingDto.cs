using Abp.AutoMapper;
using System.Collections.Generic;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.ScoreSettings.Dtos
{
    [AutoMapFrom(typeof(ScoreSetting))]
    public class ScoreSettingDto
    {
        public long Id { get; set; }
        public UserType UserType { get; set; }
        public string UserTypeName { get; set; }
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public List<ScoreRangeDto> ScoreRanges { get; set; }
    }
}
