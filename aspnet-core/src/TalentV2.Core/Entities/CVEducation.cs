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
	public class CVEducation : NccAuditEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		public long EducationId { get; set; }
		[ForeignKey(nameof(EducationId))]
		public Education Education { get; set; }
		public long CVId { get; set; }
		[ForeignKey(nameof(CVId))]
		public CV CV { get; set; }
	}
}
