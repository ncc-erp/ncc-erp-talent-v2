using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace TalentV2.WebServices.ExternalServices.Firebase
{
    public class FirebaseServices : BaseWebService, ISingletonDependency
    {
        private readonly IOptions<FirebaseConfig> _options;

        public FirebaseServices(HttpClient httpClient,
            IOptions<FirebaseConfig> options,
            ILogger<FirebaseServices> logger,
            IAbpSession session) : base(httpClient, logger, session)
        {
            _options = options;
        }

        public async Task<Dictionary<string, Applicant>> CrawlData()
        {
            string url = $"{_options.Value.Url}/career/.json?auth={_options.Value.SecretKey}";
            try
            {
                logger.LogInformation("Sending GET request to {Url}", url);
                var response = await HttpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Dictionary<string, Applicant>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Allows for case-insensitive property matching
                });
                return data;
            }
            catch (Exception ex)
            {
                // Log any exceptions that might occur
                logger.LogError("An unexpected error occurred.{Message}",ex.Message);
            }
            return null;
        }
    }

    public class Applicant
    {
        public string AboutJob { get; set; }
        public string Email { get; set; }
        public string FileURL { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string Office { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }

    }
}
