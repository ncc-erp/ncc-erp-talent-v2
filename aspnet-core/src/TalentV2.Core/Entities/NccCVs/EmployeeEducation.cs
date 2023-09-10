using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities.NccCVs
{
    public class EmployeeEducation : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long? CVEmployeeId { get; set; }
        public string SchoolOrCenterName { get; set; }
        public DegreeTypeEnum DegreeType { get; set; }
        public string Major { get; set; }
        [StringLength(4)]
        public string StartYear { get; set; }
        [StringLength(4)]
        public string EndYear { get; set; }
        public string Description { get; set; }
        public int? Order { get; set; }
        [ForeignKey(nameof(CVEmployeeId))]
        public User Cvemployee { get; set; }
    }
}
