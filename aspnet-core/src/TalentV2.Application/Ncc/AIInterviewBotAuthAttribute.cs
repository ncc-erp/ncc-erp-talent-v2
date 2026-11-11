using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Configuration;
using TalentV2.DomainServices.AIInterviews.Dtos;
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
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AIInterviewBotAuthAttribute>>();
            var traceId = context.HttpContext.TraceIdentifier;
            var sessionId = context.ActionArguments.Values.OfType<AIInterviewWebhookDto>().FirstOrDefault()?.SessionId;
            var timer = Stopwatch.StartNew();
            logger.LogInformation("[AIInterview] Webhook received. TraceId: {TraceId}, SessionId: {SessionId}", traceId, sessionId);

            try
            {
                await AuthorizeAndExecute(context, async () =>
                {
                    var executed = await next();
                    if (executed.Exception != null)
                        logger.LogError(executed.Exception,
                            "[AIInterview] Webhook action failed. TraceId: {TraceId}, SessionId: {SessionId}", traceId, sessionId);
                    return executed;
                }, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[AIInterview] Webhook failed. TraceId: {TraceId}, SessionId: {SessionId}", traceId, sessionId);
                throw;
            }
            finally
            {
                logger.LogInformation("[AIInterview] Webhook finished. TraceId: {TraceId}, SessionId: {SessionId}, ElapsedMs: {ElapsedMs}",
                    traceId, sessionId, timer.ElapsedMilliseconds);
            }
        }

        private static async Task AuthorizeAndExecute(ActionExecutingContext context, ActionExecutionDelegate next,
            ILogger<AIInterviewBotAuthAttribute> logger)
        {
            var settingManager = IocManager.Instance.Resolve<ISettingManager>();
            var tenantManager = IocManager.Instance.Resolve<TenantManager>();
            var abpSession = IocManager.Instance.Resolve<IAbpSession>();

            var header = context.HttpContext.Request.Headers;
            var secret = settingManager.GetSettingValueForApplication(AppSettingNames.AIInterviewWebhookSecret);
            var received = header["X-Secret-Key"].ToString();
            var secretNotConfigured = string.IsNullOrWhiteSpace(secret)
                || string.Equals(secret.Trim(), "yourAIInterviewWebhookSecret", StringComparison.OrdinalIgnoreCase);

            if (secretNotConfigured || !FixedTimeEquals(secret, received))
            {
                logger.LogWarning("[AIInterview] Webhook rejected (401). TraceId: {TraceId}, Reason: {Reason}",
                    context.HttpContext.TraceIdentifier, secretNotConfigured ? "Webhook secret is not configured" : "Missing or invalid secret key");
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
                logger.LogWarning("[AIInterview] Webhook rejected (400): tenant not found. TraceId: {TraceId}",
                    context.HttpContext.TraceIdentifier);
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
