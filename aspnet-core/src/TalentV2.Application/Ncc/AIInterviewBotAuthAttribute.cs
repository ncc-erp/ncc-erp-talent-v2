using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Configuration;
using TalentV2.MultiTenancy;

namespace TalentV2.Ncc
{
    /// <summary>
    /// Auth for the AI interview bot webhook: header "X-Secret-Key" must equal setting App.AIInterviewWebhookSecret.
    /// Optional header "Abp-TenantName" switches the tenant (same as <see cref="NccAuthAttribute"/>).
    /// </summary>
    public class AIInterviewBotAuthAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var settingManager = IocManager.Instance.Resolve<ISettingManager>();
            var tenantManager = IocManager.Instance.Resolve<TenantManager>();
            var abpSession = IocManager.Instance.Resolve<IAbpSession>();

            var header = context.HttpContext.Request.Headers;
            var secret = settingManager.GetSettingValueForApplication(AppSettingNames.AIInterviewWebhookSecret);
            var received = header["X-Secret-Key"].ToString();

            if (string.IsNullOrEmpty(secret) || !FixedTimeEquals(secret, received))
            {
                context.Result = new UnauthorizedObjectResult(new { success = false, message = "Invalid secret key" });
                return;
            }

            var tenantName = header["Abp-TenantName"].ToString();
            if (string.IsNullOrEmpty(tenantName))
            {
                await next();
                return;
            }

            var tenant = tenantManager.FindByTenancyName(tenantName);
            if (tenant == null)
            {
                context.Result = new BadRequestObjectResult(new { success = false, message = $"Tenant '{tenantName}' not found" });
                return;
            }

            using (abpSession.Use(tenant.Id, null))
            {
                await next();
            }
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            var bytesA = Encoding.UTF8.GetBytes(a ?? string.Empty);
            var bytesB = Encoding.UTF8.GetBytes(b ?? string.Empty);
            return bytesA.Length == bytesB.Length && CryptographicOperations.FixedTimeEquals(bytesA, bytesB);
        }
    }
}
