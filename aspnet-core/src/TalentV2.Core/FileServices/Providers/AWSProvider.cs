using Abp.Dependency;
using Amazon.S3;
using Amazon.S3.Model;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.MultiTenancy;
using TalentV2.NccCore;
using TalentV2.Utils;

namespace TalentV2.FileServices.Providers
{
    public class AWSProvider : IFileProvider
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger _logger;
        public AWSProvider(IAmazonS3 amazonS3)
        {
            _s3Client = amazonS3;
            _logger = IocManager.Instance.Resolve<ILogger>();
        }

        public async Task<string> UploadFileAsync(List<string> paths, IFormFile file)
        {
            var key = $"{string.Join("/",paths)}/{DateTimeUtils.GetNow().ToString("yyyyMMddHHmmss")}_{file.FileName}";
            _logger.Info($"UploadFileAsync() Key: {key}");
            var request = new PutObjectRequest()
            {
                BucketName = AmazoneS3Constant.BucketName,
                Key = key,
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(request);
            return key;
        }
    }
}
