using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Entities.NccCVs;

namespace TalentV2.Entities
{
	public class Skill : NccAuditEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		[MaxLength(100)]
		public string Name { get; set; }
		public long? GroupSkillId { get; set; }
		[ForeignKey(nameof(GroupSkillId))]
		public GroupSkill GroupSkill { get; set; }

		public ICollection<RequestSkill> RequestSkills { get; set; }
		public ICollection<CVSkill> CVSkills { get; set; }
	}
}
