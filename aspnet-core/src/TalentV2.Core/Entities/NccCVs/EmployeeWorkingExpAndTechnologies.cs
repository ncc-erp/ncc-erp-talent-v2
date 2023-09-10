using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities.NccCVs
{
    public class EmployeeWorkingExpAndTechnologies : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long WorkingExpId { get; set; }
        public long TechnologyId { get; set; }
        [ForeignKey(nameof(WorkingExpId))]
        public EmployeeWorkingExperience EmployeeWorkingExperiences { get; set; }
        [ForeignKey(nameof(TechnologyId))]
        public Technology Technologies { get; set; }
    }
}
