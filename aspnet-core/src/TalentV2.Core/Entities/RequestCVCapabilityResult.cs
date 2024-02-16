using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
	public class RequestCVCapabilityResult : NccAuditEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		public int Score { get; set; }
		[MaxLength(5000)]
		public string Note { get; set; }

		public long RequestCVId { get; set; }
		[ForeignKey(nameof(RequestCVId))]
		public RequestCV RequestCV { get; set; }
		public long CapabilityId { get; set; }
		[ForeignKey(nameof(CapabilityId))]
		public Capability Capability { get; set; }
		public int Factor { get; set; }
	}
}
