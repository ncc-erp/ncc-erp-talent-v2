using System.Collections.Generic;
using static TalentV2.Authorization.GrantPermissionRoles;

namespace TalentV2.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        public RoleEditDto Role { get; set; }

        public List<SystemPermission> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}