using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Configuration;
using TalentV2.MultiTenancy;

namespace TalentV2.Ncc
{
    public class MezonBotAuthAttribute : ActionFilterAttribute
    {
        private readonly ISettingManager _settingManager;
        private readonly TenantManager _tenantManager;
        private readonly IAbpSession _abpSession;
        public MezonBotAuthAttribute()
        {
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();
            _tenantManager = IocManager.Instance.Resolve<TenantManager>();
            _abpSession = NullAbpSession.Instance;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var header = context.HttpContext.Request.Headers;
            var signature = _settingManager.GetSettingValue(AppSettingNames.MezonBotSignature);
            var receivedHash = header["X-Hash"].ToString();
            if (string.IsNullOrEmpty(receivedHash))
            {
                throw new UserFriendlyException("Missing hash header");
            }

            var request = context.HttpContext.Request;
            string payload = await GetPayloadFromRequest(request);

            var expectedHash = ComputeHash(payload + signature);

            if (receivedHash != expectedHash)
            {
                throw new UserFriendlyException("Invalid hash");
            }

            await next();
        }

        private async Task<string> GetPayloadFromRequest(HttpRequest request)
        {
            string payload;

            if (request.Method == "GET")
            {
                payload = GetQueryStringData(request.Query);
            }
            else
            {
                var contentType = request.ContentType?.ToLower() ?? "";

                if (contentType.Contains("application/json") || string.IsNullOrEmpty(contentType))
                {
                    request.EnableBuffering();
                    using (var reader = new StreamReader(
                        request.Body,
                        encoding: Encoding.UTF8,
                        detectEncodingFromByteOrderMarks: false,
                        leaveOpen: true))
                    {
                        payload = await reader.ReadToEndAsync();
                        request.Body.Position = 0;
                    }
                }
                else if (contentType.Contains("multipart/form-data"))
                {
                    payload = await GetFormDataString(request.Form, request.Form.Files);
                }
                else if (contentType.Contains("application/x-www-form-urlencoded"))
                {
                    payload = await GetFormUrlEncodedString(request.Form);
                }
                else
                {
                    throw new UserFriendlyException("Unsupported content type for hash verification");
                }
            }

            return payload;
        }

        private string GetQueryStringData(IQueryCollection query)
        {
            var queryValues = new StringBuilder();
            
            foreach (var key in query.Keys.OrderBy(k => k))
            {
                var values = query[key];
                foreach (var value in values.OrderBy(v => v))
                {
                    queryValues.Append(key).Append('=').Append(value).Append('&');
                }
            }
            
            if (queryValues.Length > 0 && queryValues[queryValues.Length - 1] == '&')
            {
                queryValues.Length--;
            }
            
            return queryValues.ToString();
        }

        private async Task<string> GetFormDataString(IFormCollection form, IFormFileCollection files)
        {
            var formValues = new StringBuilder();
            
            foreach (var key in form.Keys.OrderBy(k => k))
            {
                var values = form[key];
                foreach (var value in values.OrderBy(v => v))
                {
                    formValues.Append(key).Append('=').Append(value).Append('&');
                }
            }
            
            foreach (var file in files.OrderBy(f => f.Name))
            {
                formValues.Append(file.Name).Append('=')
                          .Append(file.FileName).Append(':')
                          .Append(file.Length).Append('&');
            }
            
            if (formValues.Length > 0 && formValues[formValues.Length - 1] == '&')
            {
                formValues.Length--;
            }
            
            return formValues.ToString();
        }

        private Task<string> GetFormUrlEncodedString(IFormCollection form)
        {
            var formValues = new StringBuilder();
            
            foreach (var key in form.Keys.OrderBy(k => k))
            {
                var values = form[key];
                foreach (var value in values.OrderBy(v => v))
                {
                    formValues.Append(key).Append('=').Append(value).Append('&');
                }
            }
            
            if (formValues.Length > 0 && formValues[formValues.Length - 1] == '&')
            {
                formValues.Length--;
            }
            
            return Task.FromResult(formValues.ToString());
        }

        private string ComputeHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
