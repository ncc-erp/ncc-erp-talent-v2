using Abp.Dependency;
using Amazon.S3;
using Amazon.S3.Model;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
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
            var key = $"{string.Join("/", paths)}/{DateTimeUtils.GetNow().ToString("yyyyMMddHHmmss")}_{file.FileName}";
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

        public async Task<Stream> ReadFileAsync(List<string> paths, string fileName)
        {
            var key = fileName;
            if (paths != null && paths.Count > 0)
            {
                key = $"{string.Join("/", paths)}/{fileName}";
            }

            _logger.Info($"ReadFileAsync() Key: {key}");
            var request = new GetObjectRequest()
            {
                BucketName = AmazoneS3Constant.BucketName,
                Key = key,
            };
            var response = await _s3Client.GetObjectAsync(request);
            return response.ResponseStream;
        }

        public async Task<string> CopyFileAsync(List<string> sourcePaths, List<string> destinationPaths, string fileName, bool hasTimestamp = false)
        {
            var sourceKey = fileName;
            if (sourcePaths != null && sourcePaths.Count > 0)
            {
                sourceKey = $"{string.Join("/", sourcePaths)}/{fileName}";
            }

            if (hasTimestamp)
            {
                fileName = $"{DateTimeUtils.GetNow():yyyyMMddHHmmss}_{fileName}";
            }

            var destinationKey = fileName;
            if (destinationPaths != null && destinationPaths.Count > 0)
            {
                destinationKey = $"{string.Join("/", destinationPaths)}/{fileName}";
            }

            _logger.Info($"CopyFileAsync() SourceKey: {sourceKey} to DestinationKey: {destinationKey}");
            var copyRequest = new CopyObjectRequest()
            {
                SourceBucket = AmazoneS3Constant.BucketName,
                DestinationBucket = AmazoneS3Constant.BucketName,
                SourceKey = sourceKey,
                DestinationKey = destinationKey,
            };

            await _s3Client.CopyObjectAsync(copyRequest);

            return destinationKey;
        }

        public async Task MoveFileAsync(List<string> sourcePaths, List<string> destinationPaths, string fileName, bool hasTimestamp = false)
        {
            var sourceKey = fileName;
            if (sourcePaths != null && sourcePaths.Count > 0)
            {
                sourceKey = $"{string.Join("/", sourcePaths)}/{fileName}";
            }

            if (hasTimestamp)
            {
                fileName = $"{DateTimeUtils.GetNow():yyyyMMddHHmmss}_{fileName}";
            }

            var destinationKey = fileName;
            if (destinationPaths != null && destinationPaths.Count > 0)
            {
                destinationKey = $"{string.Join("/", destinationPaths)}/{fileName}";
            }

            _logger.Info($"MoveFileAsync() SourceKey: {sourceKey} to DestinationKey: {destinationKey}");
            var copyRequest = new CopyObjectRequest()
            {
                SourceBucket = AmazoneS3Constant.BucketName,
                DestinationBucket = AmazoneS3Constant.BucketName,
                SourceKey = sourceKey,
                DestinationKey = destinationKey,
            };

            await _s3Client.CopyObjectAsync(copyRequest);

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = AmazoneS3Constant.BucketName,
                Key = sourceKey
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

        public async Task MoveCvFileToFolderAsync(List<string> sourcePaths, string fileName, string folderName, bool hasTimestamp = false)
        {
            var sourceKey = fileName;
            if (sourcePaths != null && sourcePaths.Count > 0)
            {
                sourceKey = $"{string.Join("/", sourcePaths)}/{fileName}";
            }

            if (hasTimestamp)
            {
                fileName = $"{DateTimeUtils.GetNow():yyyyMMddHHmmss}_{fileName}";
            }

            var destinationKey = fileName;
            if (sourcePaths != null && sourcePaths.Count > 0)
            {
                destinationKey = $"{string.Join("/", sourcePaths)}/{folderName}/{fileName}";
            }

            _logger.Info($"ArchiveFileAsync() SourceKey: {sourceKey} to DestinationKey: {destinationKey}");
            var copyRequest = new CopyObjectRequest()
            {
                SourceBucket = AmazoneS3Constant.BucketName,
                DestinationBucket = AmazoneS3Constant.BucketName,
                SourceKey = sourceKey,
                DestinationKey = destinationKey,
            };

            await _s3Client.CopyObjectAsync(copyRequest);

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = AmazoneS3Constant.BucketName,
                Key = sourceKey
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

        public async Task<List<string>> GetFileNamesAsync(List<string> paths)
        {
            var prefix = $"{string.Join("/", paths)}/";
            _logger.Info($"GetFileNamesAsync() Prefix: {prefix}");
            var request = new ListObjectsV2Request()
            {
                BucketName = AmazoneS3Constant.BucketName,
                Prefix = prefix,
                MaxKeys = 100,
                Delimiter = "/"
            };
            var response = await _s3Client.ListObjectsV2Async(request);
            return response.S3Objects.Select(x => Path.GetFileName(x.Key))
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }
    }
}