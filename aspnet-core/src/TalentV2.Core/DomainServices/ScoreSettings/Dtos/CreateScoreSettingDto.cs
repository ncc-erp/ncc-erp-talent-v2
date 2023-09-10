using Abp.AutoMapper;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.ScoreSettings.Dtos
{
    [AutoMapTo(typeof(ScoreSetting))]
    public class CreateScoreSettingDto
    {
        public long SubPositionId { get; set; }
        public UserType UserType { get; set; }
        public CreateScoreRangeDto Range { get; set; }

    }
}
