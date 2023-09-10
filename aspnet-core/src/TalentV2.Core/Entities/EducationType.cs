using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
	public class EducationType : NccAuditEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		[MaxLength(100)]
		public string Name { get; set; }
		public ICollection<Education> Educations { get; set; }
	}
}
