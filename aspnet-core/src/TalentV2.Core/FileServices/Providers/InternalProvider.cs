using Abp.Dependency;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TalentV2.Utils;

namespace TalentV2.FileServices.Providers
{
    public class InternalProvider : IFileProvider
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public InternalProvider(IHostingEnvironment hostingEnvironment)
        {
            _logger = IocManager.Instance.Resolve<ILogger>();
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> UploadFileAsync(List<string> paths, IFormFile file)
        {
            var newPaths = new List<string>(paths);
            newPaths.Insert(0, _hostingEnvironment.WebRootPath);
            var path = Path.Combine(newPaths.ToArray());
            CreateIfNotExist(path);
            var fileName = $"{DateTimeUtils.GetNow().ToString("yyyyMMddHHmmss")}_{file.FileName}";
            var endPath = Path.Combine(path, fileName);

            using (var stream = File.Create(endPath))
            {
                await file.CopyToAsync(stream);
            }

            _logger.Info($"UploadFileAsync() Key: {endPath}");
            return string.Join("/", paths) + "/" + fileName;
        }

        public Task<Stream> ReadFileAsync(List<string> paths, string fileName)
        {
            throw new NotImplementedException("ReadFileAsync() method is only supported for AWS.");
        }

        public Task<string> CopyFileAsync(List<string> sourcePaths, List<string> destinationPaths, string fileName, bool hasTimestamp = false)
        {
            throw new NotImplementedException("CopyFileAsync() method is only supported for AWS.");
        }
        

        public Task MoveFileAsync(List<string> sourcePaths, List<string> destinationPaths, string fileName, bool hasTimestamp = false)
        {
            throw new NotImplementedException("MoveFileAsync() method is only supported for AWS.");
        }

        public Task MoveCvFileToFolderAsync(List<string> sourcePaths, string fileName, string folderName, bool hasTimestamp = false)
        {
            throw new NotImplementedException("ArchiveFileAsync() method is only supported for AWS.");
        }

        public Task<List<string>> GetFileNamesAsync(List<string> paths)
        {
            throw new NotImplementedException("GetFileNamesAsync() method is only supported for AWS.");
        }

        private void CreateIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}