using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;

namespace TalentV2.Entities.NccCVs
{
    public class EmployeeWorkingExperience : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long UserId { get; set; }
        public long? ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Position { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string Responsibilities { get; set; }
        public string Technologies { get; set; }
        public int? Order { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public long? VersionId { get; set; }
        [ForeignKey(nameof(VersionId))]
        public Versions Version { get; set; }
    }
}
