using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.FileServices.Paths;
using TalentV2.FileServices.Providers;
using TalentV2.Utils;

namespace TalentV2.FileServices.Services.Candidates
{
    public class FileCandidateService : IFileCandidateService
    {
        private readonly string FOLDER_SERVICE = "candidates";
        private readonly IAbpSession _session;
        private readonly IFileProvider _fileService;
        private readonly IFilePath _filePath;
        public FileCandidateService(IAbpSession session, IFileProvider fileService, IFilePath filePath)
        {
            _session = session;
            _fileService = fileService;
            _filePath = filePath;
        }
        public async Task<string> UploadCV(IFormFile file)
        {
            CommonUtils.CheckSizeFile(file);
            CommonUtils.CheckFormatFile(file, FileTypes.DOCUMENT);

            var paths = await _filePath.GetPath(FOLDER_SERVICE, PathFolder.FOLDER_CV, _session.TenantId);
            var subUrl = await _fileService.UploadFileAsync(paths, file);

            return subUrl;
        }
        public async Task<string> UploadAvatar(IFormFile file)
        {
            CommonUtils.CheckSizeFile(file);
            CommonUtils.CheckFormatFile(file, FileTypes.IMAGE);

            var paths = await _filePath.GetPath(FOLDER_SERVICE, PathFolder.FOLDER_AVATAR, _session.TenantId);
            var subUrl = await _fileService.UploadFileAsync(paths, file);

            return subUrl;
        }

        public async Task<IFormFile> DownloadFileAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            string tempFilePath = Path.GetTempFileName();
            FileStream fileStream = null;

            try
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
                {
                    throw new ArgumentException("Invalid URL format");
                }

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(5);

                    using (var headResponse = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, uri)))
                    {
                        headResponse.EnsureSuccessStatusCode();

                        if (headResponse.Content.Headers.ContentLength.HasValue)
                        {
                            long fileSize = headResponse.Content.Headers.ContentLength.Value;
                            if (fileSize > 104857600)
                            {
                                throw new Exception("File too large (max 100MB)");
                            }
                        }
                    }

                    using (var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            byte[] buffer = new byte[8192]; // 8KB buffer
                            int bytesRead;
                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                            }
                        }

                        fileStream.Close();
                        fileStream.Dispose();
                        fileStream = null;

                        string fileName = null;

                        if (response.Content.Headers.ContentDisposition != null)
                        {
                            fileName = response.Content.Headers.ContentDisposition.FileName?.Trim('"');
                        }

                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            fileName = Path.GetFileName(uri.LocalPath);
                        }


                        fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

                        string contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

                        fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);
                        var fileInfo = new FileInfo(tempFilePath);

                        return new FormFile(
                            fileStream,
                            0,
                            fileInfo.Length,
                            "file",
                            fileName
                        )
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = contentType
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }

                if (File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch {  }
                }

                return null;
            }
        }
    }
}
