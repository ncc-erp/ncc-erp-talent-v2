using Abp.AutoMapper;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.ScoreSettings.Dtos
{
    [AutoMapTo(typeof(ScoreRange))]
    public class CreateScoreRangeDto
    {
        public long SubPositionId { get; set; }
        public UserType UserType { get; set; }
        public float ScoreFrom { get; set; }
        public float ScoreTo { get; set; }
        public int Level { get; set; }
    }
}
