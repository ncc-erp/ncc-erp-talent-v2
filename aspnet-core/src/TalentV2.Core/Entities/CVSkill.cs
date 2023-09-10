using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using TalentV2.Constants.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
	public class CVSkill : NccAuditEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		public Level Level { get; set; }
		[MaxLength(500)]
		public string Note { get; set; }

        public long SkillId { get; set; }
        [ForeignKey(nameof(SkillId))]
		public Skill Skill { get; set; }

		public long CVId { get; set; }
		[ForeignKey(nameof(CVId))]
		public CV CV { get; set; }
	}
}
