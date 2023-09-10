using System.Linq;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.Utils;

namespace TalentV2.DomainServices.ScoreSettings.Dtos
{
    public class ScoreRangeDto
    {
        public long Id { get; set; }
        public float ScoreFrom { get; set; }
        public float ScoreTo { get; set; }
        public Level Level { get; set; }
        public LevelDto LevelInfo { get => CommonUtils.ListLevel.FirstOrDefault(s => s.Id == Level.GetHashCode()); }
    }
}
