using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class PositionSetting : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get ; set ; }

        public UserType UserType { get; set; }
        public long SubPositionId { get; set; }
        [ForeignKey(nameof(SubPositionId))]
        public SubPosition SubPosition { get; set; }

        [MaxLength(500)]
        public string LMSCourseName { get; set; }
        public Guid? LMSCourseId { get; set; }

        [MaxLength(500)]
        public string ProjectInfo { get; set; }
        [MaxLength(500)]
        public string DiscordInfo { get; set; }
        [MaxLength(500)]
        public string IMSInfo { get; set; }
    }
}
