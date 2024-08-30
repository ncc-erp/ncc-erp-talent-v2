using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.Constants.Dictionary;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.DomainServices.Dto;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Sockets;
using TalentV2.Constants.Const;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.FileServices.Paths;
using TalentV2.FileServices.Providers;

namespace TalentV2.WebServices.ExternalServices.Autobot
{
    public class AutobotService : BaseWebService
    {
        private double _sleepTime = 5;
        private const string ExtractV2 = "extract-cv-vision";
        private const string ExtractCV = "extract-cv";
        private readonly IConfiguration _configuration;


        public AutobotService(HttpClient httpClient,
            ILogger<AutobotService> logger,
            IAbpSession abpSession,
            IConfiguration configuration
         ) : base(httpClient, logger, abpSession)
        {
            _configuration = configuration;

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

            int attempt = 1;
            bool isComplete = false;
            while (attempt <= 3 && !isComplete)
            {
                try
                {
                    using var content = new MultipartFormDataContent();
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    content.Add(streamContent, "file", fileName);

                    var response = await HttpClient.PostAsync(fullUrl, content);
                    int statusCode = (int)response.StatusCode;

                    if (response.IsSuccessStatusCode)
                    {
                        isComplete = true;
                        var responseContent = await response.Content.ReadAsStringAsync();
                        logger.LogInformation($"Post: {fullUrl} response: {responseContent}");
                        return JsonConvert.DeserializeObject<T>(responseContent);
                    }
                    else if (statusCode == 429 || statusCode >= 500)
                    {
                        attempt++;
                        Thread.Sleep(TimeSpan.FromSeconds(_sleepTime));
                    }
                    else isComplete = true;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Post: {fullUrl} error: {ex.Message}");
                    attempt++;
                    Thread.Sleep(TimeSpan.FromSeconds(_sleepTime));
                }
                finally
                {
                    Thread.Sleep(TimeSpan.FromSeconds(_sleepTime));
                }
            }
            return default;
        }




        public async Task<Dictionary<string, CVScanResultFromFireBase>> ValidateAndExtractCVFromFirebaseAsync(string fileUrl)
        {
            var result = new Dictionary<string, CVScanResultFromFireBase>();
            try
            {
                var maxFileSize = _configuration.GetSection("UploadFile:MaxSizeFile").Get<int>();
                var fileNameDecoded = WebUtility.UrlDecode(Path.GetFileName(new Uri(fileUrl).AbsolutePath));
                var fileExtension = Path.GetExtension(fileNameDecoded).Replace(".", "").ToLower();
                var httpResponse = await HttpClient.GetAsync(fileUrl);
                httpResponse.EnsureSuccessStatusCode();
                var fileBytes = await httpResponse.Content.ReadAsByteArrayAsync();
                var fileSize = fileBytes.Length;
                if (fileSize > maxFileSize)
                {
                    logger.LogError($"CV 's size is over the limit {maxFileSize / (1024 * 1024)}MB");
                    result.Add(FirebaseLogStatusConstant.CV_SIZE_TOO_BIG, null);
                    return result;
                }
                if (!DictionaryHelper.FileTypeDic[FileTypes.DOCUMENT].Contains(fileExtension))
                {
                    logger.LogError("CV 's type is not accepted. Only file type PDF, DOC, DOCX is accepted");
                    result.Add(FirebaseLogStatusConstant.CV_ERROR_TYPE, null);
                    return result;
                }

                var cvExtractionResult = await SendFileToCVExtractionAsync(fileNameDecoded, fileBytes);
                cvExtractionResult.CVData = fileBytes;
                result.Add("OK", cvExtractionResult);
                return result;
            }
            catch (Exception e)
            {
                logger.LogError("Error occurred: {Exception}", e.Message);
                return null;
            }
        }

        private async Task<CVScanResultFromFireBase> SendFileToCVExtractionAsync(string fileName, byte[] fileBytes)
        {
            var requestUrl = $"{HttpClient.BaseAddress}{ExtractV2}";
            const int maxRetries = 5;
            int delayTime = 10000; // 10s
            int attempt = 0;
            bool isComplete = false;
            while (attempt < maxRetries && !isComplete)
            {
                try
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new ByteArrayContent(fileBytes), "file", fileName);
                        var response = await HttpClient.PostAsync(requestUrl, content);
                        if (response.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            attempt++;
                            if (attempt == maxRetries)
                            {
                                logger.LogError($"Attempt {attempt} is maximum number of retries");
                                break;
                            }
                            logger.LogError($"Attempt {attempt} failed due to AI request-limiting. Retrying in {delayTime / 1000} seconds...");
                            delayTime = Math.Min(delayTime * 2, 60000);
                            await Task.Delay(delayTime);
                        }
                        else
                        {
                            response.EnsureSuccessStatusCode();
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            isComplete = true;
                            return JsonConvert.DeserializeObject<CVScanResultFromFireBase>(jsonResponse);
                        }
                    };
                }
                catch (Exception ex)
                {
                    logger.LogError("Exception occurred: {Exception}", ex.Message);
                }
            }
            logger.LogError($"Failed to extract CV from Firebase at {fileName}");
            return null;
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

        public async Task<GetResultConnectDto> CheckConnectToAutoBot()
        {
            var fullUrl = $"{HttpClient.BaseAddress}/check-connection";
            try
            {
                logger.LogInformation($"Get: {fullUrl}");
                var response = await HttpClient.GetAsync(fullUrl);
                var isSuccess = response.StatusCode == HttpStatusCode.OK;
                return new GetResultConnectDto
                {
                    IsConnected = isSuccess,
                    Message = isSuccess ? "Success" : "Can not connect to AutoBot"
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Get: {fullUrl} error: {ex.Message}");
            }
            return default;
        }
    }
}