using Abp.AutoMapper;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.ScoreSettings.Dtos
{
    [AutoMapTo(typeof(ScoreRange))]
    public class UpdateScoreRangeDto
    {
        public long ScoreRangeId { get; set; }
        public float ScoreFrom { get; set; }
        public float ScoreTo { get; set; }
        public Level Level { get; set; }
    }
}
