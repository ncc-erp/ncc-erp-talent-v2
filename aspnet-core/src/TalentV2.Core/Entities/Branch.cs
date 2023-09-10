using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
    public class Branch : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(20)]
        public string DisplayName { get; set; }
        [MaxLength(20)]
        public string ColorCode { get; set; }
        [MaxLength(250)]
        public string Address { get; set; }
        public ICollection<Request> Requests { get; set; }
        public ICollection<CV> CVs { get; set; }
    }
}
