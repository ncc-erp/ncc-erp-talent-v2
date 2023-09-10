using Abp;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Configuration;
using TalentV2.Constants.Const;
using TalentV2.MultiTenancy;

namespace TalentV2.Ncc
{
    public class NccAuthAttribute : ActionFilterAttribute
    {
        private readonly ISettingManager _settingManager;
        private readonly TenantManager _tenantManager;
        private readonly IAbpSession _abpSession;
        public NccAuthAttribute()
        {
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();
            _tenantManager = IocManager.Instance.Resolve<TenantManager>();
            _abpSession = NullAbpSession.Instance;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var secretCode = _settingManager.GetSettingValue(AppSettingNames.TalentSecurityCode);
            var header = context.HttpContext.Request.Headers;
            var securityCodeHeader = header["X-Secret-Key"].ToString();
            if (secretCode != securityCodeHeader)
                throw new UserFriendlyException($"SecretCode does not match! TalentCode: {secretCode.Substring(secretCode.Length - 3)} != {securityCodeHeader}");

            var abpTenantName = header["Abp-TenantName"].ToString();
            if (string.IsNullOrEmpty(abpTenantName)) return;

            var tenant = _tenantManager.FindByTenancyName(abpTenantName);
            if (tenant == null) 
                throw new Exception($"Not Found Tenant.");

            _abpSession.Use(tenant.Id, null);
        }
    }
}
