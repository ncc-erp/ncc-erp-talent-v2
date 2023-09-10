using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;

namespace TalentV2.Entities
{
    public abstract class NccAuditEntity : FullAuditedEntity<long>
    {
        [ForeignKey(nameof(CreatorUserId))]
        public User CreatorUser { get; set; }
        [ForeignKey(nameof(LastModifierUserId))]
        public User LastModifierUser { get; set; }
    }
}
