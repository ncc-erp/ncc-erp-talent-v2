using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Roles.Dto
{
    public class UserRoleDto
    {
        [Required]
        public List<long> UserIds { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
    public class DeleteUserRoleDto
    {
        public List<long> UserRoleIds { get; set;}
    }
}
