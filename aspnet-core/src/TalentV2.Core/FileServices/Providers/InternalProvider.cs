using Abp.Dependency;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            return string.Join("/",paths) + "/" + fileName;
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
