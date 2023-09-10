using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities.NccCVs
{
    public class Technology : FullAuditedEntity<long>, IMayHaveTenant
    {
        public string TechnologyName { get; set; }
        public string Version { get; set; }
        public int? TenantId { get; set; }
    }
}
