using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TalentV2.Entities;
using TalentV2.WebServices;

namespace TalentV2.Firebase
{
    public class FirebaseService : BaseWebService, ISingletonDependency
    {
        private readonly IOptions<FirebaseConfig> _options;
        private readonly ILogger<FirebaseService> _logger;
        private readonly IRepository<FirebaseCareerLog, int> _repositoryCareerLog;
        private readonly IUnitOfWorkManager _unitOfWorkManager;



        public FirebaseService(HttpClient httpClient,
            IOptions<FirebaseConfig> options,
            ILogger<FirebaseService> logger,
            IRepository<FirebaseCareerLog, int> repositoryCareerLog,
            IUnitOfWorkManager unitOfWorkManager,
            IAbpSession session) : base(httpClient, logger, session)
        {
            _options = options;
            _logger = logger;
            _repositoryCareerLog = repositoryCareerLog;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task<Dictionary<string, Applicant>> GetCrawl()
        {
            string url = $"{_options.Value.Url}/career/.json?auth={_options.Value.SecretKey}";
            return await this.GetAsync(url);
        }
        private async Task<Dictionary<string, Applicant>> GetAsync(string url)
        {
            List<FirebaseCareerLog> logList = new List<FirebaseCareerLog>();
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                logList = await _repositoryCareerLog.GetAllListAsync();
            };
            try
            {
                _logger.LogInformation("Sending GET request to {Url}", url);
                var response = await HttpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Dictionary<string, Applicant>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Allows for case-insensitive property matching
                });

                if (logList.IsNullOrEmpty())
                {
                    return data;
                }
                else
                {
                    var logIdSet = new HashSet<string>(logList.Select(l => l.IdFirebase));
                    foreach (var item in data)
                    {

                        if (logIdSet.Contains(item.Key))
                        {
                            data.Remove(item.Key);
                        }
                    }
                    return data;
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Log HTTP-specific exceptions
                _logger.LogError(httpEx, "An HTTP error occurred while sending the GET request.");
                throw;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "A JSON deserialization error occurred.");
                throw;
            }
            catch (Exception ex)
            {
                // Log any other exceptions that might occur
                _logger.LogError(ex, "An unexpected error occurred.");
                throw;
            }
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
    }
}
