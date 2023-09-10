using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace TalentV2.Controllers
{
    public abstract class TalentV2ControllerBase: AbpController
    {
        protected TalentV2ControllerBase()
        {
            LocalizationSourceName = TalentV2Consts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
