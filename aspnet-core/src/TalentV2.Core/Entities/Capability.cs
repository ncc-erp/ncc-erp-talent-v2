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
	public class Capability : NccAuditEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		[MaxLength(2000)]
		public string Name { get; set; }
		[MaxLength(5000)]
		public string Note { get; set; }
		[Required]
		public bool FromType { get; set; }
		public ICollection<CapabilitySetting> CapabilitySettings { get; set; }
		public ICollection<RequestCVCapabilityResult> RequestCVCapabilityResults { get; set; }
	}
}
