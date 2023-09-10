using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities.NccCVs
{
    public class Project : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public ProjectType Type { get; set; }
        public string Technology { get; set; }
        public string Description { get; set; }
    }
    public enum ProjectType
    {
        All = -1,
        CreatebyPM = 0,
        CreatebyUser = 1
    }
}
