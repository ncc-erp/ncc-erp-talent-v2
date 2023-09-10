using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using TalentV2.Constants.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
	public class RequestCVStatusHistory : NccAuditEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		public RequestCVStatus Status { get; set; }
		public DateTime TimeAt { get; set; }

		public long RequestCVId { get; set; }
		[ForeignKey(nameof(RequestCVId))]
		public RequestCV RequestCV { get; set; }
	}
}
