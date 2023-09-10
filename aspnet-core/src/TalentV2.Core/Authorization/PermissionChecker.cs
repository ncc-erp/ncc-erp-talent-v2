using Abp.Authorization;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;

namespace TalentV2.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
