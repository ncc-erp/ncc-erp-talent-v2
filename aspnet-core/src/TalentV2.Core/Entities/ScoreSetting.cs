using Abp.Domain.Entities;
using System.Collections.Generic;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class ScoreSetting : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public UserType UserType { get; set; }
        public SubPosition SubPosition { get; set; }
        public ICollection<ScoreRange> ScoreRanges { get; set; }
    }
}
