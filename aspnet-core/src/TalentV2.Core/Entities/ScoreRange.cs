using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class ScoreRange : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public float ScoreFrom { get; set; }
        public float ScoreTo { get; set; }
        public long ScoreSettingID { get; set; }
        public Level Level { get; set; }
        [ForeignKey(nameof(ScoreSettingID))]
        public ScoreSetting ScoreSetting { get; set; }
    }
}
