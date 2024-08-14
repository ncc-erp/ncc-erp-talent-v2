using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
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

        private const string ExtractCV = "extract-cv";
        private readonly string FILE_CANDIDATE_FOLDER_SERVICE = "candidates";

        private readonly IConfiguration _configuration;
        private readonly IAbpSession _session;
        private readonly IFileProvider _fileService;
        private readonly IFilePath _filePath;


        public AutobotService(HttpClient httpClient,
            ILogger<AutobotService> logger,
            IAbpSession abpSession,
            IConfiguration configuration,
            IFileProvider fileService,
            IFilePath filePath) : base(httpClient, logger, abpSession)
        {
            _configuration = configuration;
            _session = abpSession;
            _fileService = fileService;
            _filePath = filePath;
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




        public async Task<CVScanResultFromFireBase> ExtractCVFromFirebaseAsync(string fileUrl)
        {
            var fileName = Path.GetFileName(new Uri(fileUrl).AbsolutePath);
            var fileNameDecoded = WebUtility.UrlDecode(fileName);
            var fileBytes = await HttpClient.GetByteArrayAsync(fileUrl);


            var requestUrl = $"{HttpClient.BaseAddress}{ExtractCV}";
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
                        content.Add(new ByteArrayContent(fileBytes), "file", fileNameDecoded);
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
                            var result = JsonConvert.DeserializeObject<CVScanResultFromFireBase>(jsonResponse);
                            isComplete = true;
                            return result;
                        }
                    };

                }

                catch (TaskCanceledException e)
                {

                    logger.LogError("TaskCanceledException occurred: {Exception}", e);
                }
                catch (HttpRequestException ex)
                {
                    logger.LogError("HttpRequestException occurred: {Exception}", ex);

                    if (ex.InnerException is SocketException socketEx)
                    {
                        logger.LogError($"SocketException: {socketEx.Message}");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Exception occurred: {Exception}", ex);

                }
            }

            logger.LogError($"Failed to extract CV from Firebase at {fileName}");
            return null;
        }
        public async Task<bool> IsAcceptedSize(string fileUrl)
        {

            try
            {
                var response = await HttpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
                var fileSize = response.Content.Headers.ContentLength;

                var MaxFileSize = _configuration.GetSection("FirebaseConfig:FileSize").Get<int>();
                if (fileSize > MaxFileSize)
                {


                    logger.LogError($"CV 's size is over the limit {MaxFileSize / (1024 * 1024)}MB");
                    return false;
                }
            }
            catch (TaskCanceledException e)
            {
                logger.LogError("TaskCanceledException occurred: {Exception}", e);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError("HttpRequestException occurred: {Exception}", ex);

                if (ex.InnerException is SocketException socketEx)
                {
                    logger.LogError($"SocketException: {socketEx.Message}");
                }


            }

            return true;
        }
        public async Task<bool> IsAcceptedType(string fileUrl)
        {

            try
            {
                var response = await HttpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
                var contentType = response.Content.Headers.ContentType.ToString();
                var validContentTypes = new HashSet<string>
                {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

                if (!validContentTypes.Contains(contentType))
                {


                    logger.LogError($"CV 's type is not accepted. Only file type PDF, DOC, DOCX is accepted.");
                    return false;
                }
            }
            catch (TaskCanceledException e)
            {
                logger.LogError("TaskCanceledException occurred: {Exception}", e);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError("HttpRequestException occurred: {Exception}", ex);

                if (ex.InnerException is SocketException socketEx)
                {
                    logger.LogError($"SocketException: {socketEx.Message}");
                }


            }

            return true;

        }
        public bool IsAcceptedExtension(string fileUrl)
        {
            var fileName = Path.GetFileName(new Uri(fileUrl).AbsolutePath);
            var fileNameDecoded = WebUtility.UrlDecode(fileName);

            var validExtensions = new HashSet<string> { ".pdf", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(fileNameDecoded).ToLower();

            if (!validExtensions.Contains(fileExtension))
            {
                logger.LogError($"CV 's type is not accepted. Only file type PDF, DOC, DOCX is accepted.");
                return false;
            }
            return true;
        }

        public async Task<string> SaveCVToAWS(string fileURL)
        {

            var fileName = Path.GetFileName(new Uri(fileURL).AbsolutePath);
            var fileNameDecoded = WebUtility.UrlDecode(fileName);
            var fileBytes = await HttpClient.GetByteArrayAsync(fileURL);
            var streamConverted = new MemoryStream(fileBytes);
            var formFile = CreateFormFile(fileBytes, fileNameDecoded, streamConverted);
            var paths = await _filePath.GetPath(FILE_CANDIDATE_FOLDER_SERVICE, PathFolder.FOLDER_CV, _session.TenantId);

            var cvAwslink = await _fileService.UploadFileAsync(paths, formFile);
            return cvAwslink;


        }
        private static IFormFile CreateFormFile(byte[] fileBytes, string fileName, MemoryStream stream)
        {
            return new FormFile(stream, 0, fileBytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream" // 
            };
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