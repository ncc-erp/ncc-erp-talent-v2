using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TalentV2.MultiTenancy;

namespace TalentV2.WebServices
{
    public class BaseWebService
    {
        private readonly HttpClient httpClient;
        public HttpClient HttpClient
        {
            get { return httpClient; }
        }

        protected readonly ILogger logger;
        private readonly IAbpSession _abpSession;
        private readonly TenantManager _tenantManager;

        public BaseWebService(
            HttpClient httpClient,
            ILogger logger,
            IAbpSession abpSession
        )
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this._abpSession = abpSession;
            this._tenantManager = IocManager.Instance.Resolve<TenantManager>();
            SetHeader();
        }

        protected virtual async Task<T> GetAsync<T>(string url)
        {
            var fullUrl = $"{httpClient.BaseAddress}/{url}";
            try
            {
                logger.LogInformation($"Get: {fullUrl}");
                var response = await httpClient.GetAsync(url);

                /*  if (response.IsSuccessStatusCode)
                  {*/
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogInformation($"Get: {fullUrl} response: {responseContent}");
                return JsonConvert.DeserializeObject<T>(responseContent);
                /* }*/
            }
            catch (Exception ex)
            {
                logger.LogError($"Get: {fullUrl} error: {ex.Message}");
            }

            return default;
        }

        protected virtual async Task<T> PostAsync<T>(string url, object input)
        {
            var fullUrl = $"{httpClient.BaseAddress}/{url}";
            var strInput = JsonConvert.SerializeObject(input);
            var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(url, contentString);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    logger.LogInformation($"Post: {fullUrl} input: {strInput} response: {responseContent}");
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Post: {fullUrl} error: {ex.Message}");
            }
            return default;
        }

        protected virtual void Post(string url, object input)
        {
            var fullUrl = $"{httpClient.BaseAddress}/{url}";
            string strInput = JsonConvert.SerializeObject(input);
            try
            {
                logger.LogInformation($"Post: {fullUrl} input: {strInput}");
                var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");
                httpClient.PostAsync(url, contentString);
            }
            catch (Exception e)
            {
                logger.LogError($"Post: {fullUrl} input: {strInput} Error: {e.Message}");
            }
        }

        protected virtual string GetTenantName()
        {
            if (!_abpSession.TenantId.HasValue) return string.Empty;
            var tenant = _tenantManager.FindById(_abpSession.TenantId.Value);
            return tenant.TenancyName;
        }

        private void SetHeader()
        {
            this.httpClient.DefaultRequestHeaders.Add("Abp-TenantName", GetTenantName());
        }
    }
}