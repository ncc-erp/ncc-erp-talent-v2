using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
	public class RequestSkill : NccAuditEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }
        public long RequestId { get; set; }
        [ForeignKey(nameof(RequestId))]
		public Request Request { get; set; }
		public long SkillId { get; set; }
		[ForeignKey(nameof(SkillId))]
		public Skill Skill { get; set; }

	}
}
