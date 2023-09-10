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
    public class Versions : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string VersionName { get; set; }
        public long PositionId { get; set; }
        [ForeignKey(nameof(PositionId))]
        public EmployeePosition Position { get; set; }
        public long? LanguageId { get; set; }
        [ForeignKey(nameof(LanguageId))]
        public EmployeeLanguage Language { get; set; }
        public long EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public User Employee { get; set; }
    }
}
