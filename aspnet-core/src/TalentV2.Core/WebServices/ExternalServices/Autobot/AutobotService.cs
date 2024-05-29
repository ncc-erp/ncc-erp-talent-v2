using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TalentV2.WebServices.ExternalServices.Autobot
{
    public class AutobotService : BaseWebService
    {
        private double _sleepTime = 5;

        private const string ExtractCV = "extract-cv";

        public AutobotService(HttpClient httpClient, ILogger<AutobotService> logger, IAbpSession abpSession) : base(httpClient, logger, abpSession)
        {
        }

        public async Task<T> ExtractCVInformationAsync<T>(IFormFile file)
        {
            var fullUrl = $"{HttpClient.BaseAddress}{ExtractCV}";
            var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);

            try
            {
                var content = new MultipartFormDataContent
                {
                    { new StreamContent(file.OpenReadStream()), "file", file.FileName }
                };
                request.Content = content;
                var response = await HttpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    logger.LogInformation($"Post: {fullUrl} input: {file.FileName} response: {responseContent}");
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Post: {fullUrl} error: {ex.Message}");
            }
            finally
            {
                Thread.Sleep(TimeSpan.FromSeconds(_sleepTime));
            }
            return default;
        }

        public async Task<T> ExtractCVInformationAsync<T>(Stream stream, string fileName)
        {
            var fullUrl = $"{HttpClient.BaseAddress}{ExtractCV}";

            try
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(streamContent, "file", fileName);

                int retry = 1;
                bool isComplete = false;
                while (retry <= 3 && !isComplete)
                {
                    var response = await HttpClient.PostAsync(fullUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        isComplete = true;
                        var responseContent = await response.Content.ReadAsStringAsync();
                        logger.LogInformation($"Post: {fullUrl} response: {responseContent}");
                        return JsonConvert.DeserializeObject<T>(responseContent);
                    }
                    else if (response.StatusCode.GetHashCode() == 429 || response.StatusCode.GetHashCode() >= 500) retry++;
                    else isComplete = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Post: {fullUrl} error: {ex.Message}");
            }
            finally
            {
                Thread.Sleep(TimeSpan.FromSeconds(_sleepTime));
            }
            return default;
        }

        public void SetSleepTime(double seconds)
        {
            if (seconds < 5)
            {
                logger.LogInformation("AutobotService: _sleepTime will default to 5 seconds to protect the system.");
                _sleepTime = 5;
            }
            _sleepTime = seconds;
        }
    }
}