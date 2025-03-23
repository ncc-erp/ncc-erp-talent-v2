using Abp.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NccCore.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TalentV2.WebServices.ExternalServices.Komu;
using TalentV2.WebServices.ExternalServices.Mezon.Dtos;

namespace TalentV2.WebServices.ExternalServices.Mezon
{
    public class MezonService : BaseWebService
    {
        private const string serviceName = "MezonService";
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly string _appId;
        private readonly string _appToken;

        public MezonService(HttpClient httpClient, IConfiguration configuration, ILogger<KomuService> logger, IAbpSession abpSession)
            : base(httpClient, logger, abpSession)
        {
            _clientId = configuration.GetValue<string>($"{serviceName}:ClientId");
            _clientSecret = configuration.GetValue<string>($"{serviceName}:ClientSecret");
            _redirectUri = configuration.GetValue<string>($"{serviceName}:RedirectUri");
            _appId = configuration.GetValue<string>($"{serviceName}:AppId");
            _appToken = configuration.GetValue<string>($"{serviceName}:AppToken");
        }

        protected override async Task<T> PostAsync<T>(string url, object input)
        {
            var fullUrl = $"{HttpClient.BaseAddress}/{url}";

            try
            {
                var body = new FormUrlEncodedContent(input as Dictionary<string, string>);

                var response = await HttpClient.PostAsync(url, body);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    logger.LogInformation($"Post: {fullUrl} input: {body} response: {responseContent}");
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Post: {fullUrl} error: {ex.Message}");
            }
            return default;
        }

        public async Task<OAuth2TokenResponse> GetTokenAsync(OAuth2Request request)
        {
            return await PostAsync<OAuth2TokenResponse>("oauth2/token", new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", request.Code },
                { "scope", request.Scope },
                { "state", request.State },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "redirect_uri", _redirectUri }
            });
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            var body = new Dictionary<string, string>()
            {
                { "access_token", Uri.EscapeDataString(accessToken) },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "redirect_uri", _redirectUri }
            };
            return await PostAsync<UserInfoResponse>("userinfo", body);
        }

        public string GenerateOAuthUrl()
        {
            var state = Guid.NewGuid().ToString("N").Truncate(11);

            return $"{HttpClient.BaseAddress}/oauth2/auth?" +
                   $"client_id={_clientId}&" +
                   $"redirect_uri={Uri.EscapeDataString(_redirectUri)}&" +
                   $"response_type=code&" +
                   $"scope=openid+offline&" +
                   $"state={state}";
        }

        public MezonServiceConfig GetConfig()
        {
            return new MezonServiceConfig
            {
                ServiceName = serviceName,
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                RedirectUri = _redirectUri,
                AppId = _appId,
                AppToken = _appToken
            };
        }
    }
}
