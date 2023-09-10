using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;
using static TalentV2.Authorization.GrantPermissionRoles;

namespace TalentV2.Authorization
{
    public class TalentV2AuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            foreach (var permission in SystemPermission.ListPermissions)
            {
                context.CreatePermission(permission.Name, L(permission.DisplayName), multiTenancySides: permission.MultiTenancySides);
            }
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, TalentV2Consts.LocalizationSourceName);
        }
    }
}
