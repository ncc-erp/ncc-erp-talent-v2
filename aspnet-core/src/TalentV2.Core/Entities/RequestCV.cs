using Abp.Domain.Entities;
using TalentV2.Constants.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentV2.Entities
{
	public class RequestCV : NccAuditEntity, IMayHaveTenant
	{
        public int? TenantId { get; set; }
		public RequestCVStatus Status { get; set; }
		public DateTime? InterviewTime { get; set; }
		public Level? ApplyLevel { get; set; }
		public Level? InterviewLevel { get; set; }
		public Level? FinalLevel { get; set; }
		[MaxLength(1000)]
		public string HRNote { get; set; }
		public DateTime? OnboardDate { get; set; }
		public float Salary { get; set; }
		[MaxLength(5000)]
		public string LMSInfo { get; set; }

        public long CVId { get; set; }
		[ForeignKey(nameof(CVId))]
		public CV CV { get; set; }
		public long RequestId { get; set; }
		[ForeignKey(nameof(RequestId))]
		public virtual Request Request { get;set;}

		public virtual ICollection<RequestCVInterview> RequestCVInterviews { get; set; }
		public virtual ICollection<RequestCVCapabilityResult> RequestCVCapabilityResults { get; set; }
		public virtual ICollection<RequestCVStatusHistory> RequestCVStatusHistories { get; set; }
        public virtual ICollection<RequestCVStatusChangeHistory> RequestCVStatusChangeHistoies { get; set; }
        public bool? EmailSent { get; set; }
        public bool? Interviewed { get; set; }
        public string Percentage { get; set; }
    }
}
