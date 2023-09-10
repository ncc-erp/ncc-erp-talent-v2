using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
	public class CVSource : NccAuditEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		[MaxLength(100)]
		public string Name { get; set; }
		public CVSourceReferenceType? ReferenceType { get; set; }
		[MaxLength(20)]
		public string ColorCode { get; set; }
		public ICollection<CV> CVs { get; set; }
	}
}
