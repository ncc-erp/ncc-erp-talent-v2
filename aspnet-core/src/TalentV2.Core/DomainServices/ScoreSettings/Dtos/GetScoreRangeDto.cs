using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.ScoreSettings.Dtos
{
    public class GetScoreRangeDto
    {
        public UserType UserType { get; set; }
        public long SubPositionId { get; set; }
    }
}
