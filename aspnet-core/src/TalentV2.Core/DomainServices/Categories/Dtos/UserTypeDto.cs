using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Categories.Dtos
{
    public class UserTypeDto : Entity<long>
    {
        public string UserTypeName { get; set; }
    }
}
