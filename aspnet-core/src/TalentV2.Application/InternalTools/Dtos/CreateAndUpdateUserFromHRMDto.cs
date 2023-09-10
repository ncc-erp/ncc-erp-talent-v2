using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;

namespace TalentV2.InternalTools.Dtos
{
    [AutoMapTo(typeof(User))]
    public class CreateUserFromHRMDto
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
    }
    [AutoMapFrom(typeof(User))]
    public class UpdateUserFromHRMDto
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
    }
}
