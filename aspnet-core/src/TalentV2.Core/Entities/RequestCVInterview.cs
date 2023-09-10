using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using TalentV2.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
	public class RequestCVInterview : FullAuditedEntity<long>, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		public long RequestCVId { get; set; }
		[ForeignKey(nameof(RequestCVId))]
		public virtual RequestCV RequestCV { get; set; }
        public long InterviewId { get; set; }
        [ForeignKey(nameof(InterviewId))]
		public virtual User Interview { get; set; }
	}
}
