using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.FileServices.Paths;
using TalentV2.FileServices.Providers;
using TalentV2.MultiTenancy;
using TalentV2.Utils;

namespace TalentV2.UploadFilesService
{
    public class UploadAvatarService : ITransientDependency
    {
        private readonly string FOLDER_SERVICE = "avatarUser";
        private readonly IFileProvider _fileProvider;
        private readonly ILogger<UploadAvatarService> _logger;

        private readonly IAbpSession _session;
        private readonly IFilePath _filePath;
        public UploadAvatarService(IFileProvider fileProvider, ILogger<UploadAvatarService> logger, IAbpSession session, IFilePath filePath)
        {
            _fileProvider = fileProvider;
            _filePath = filePath;
            _logger = logger;
            _session = session;
            _filePath = filePath;
        }

        public async Task<string> UploadAvatarAsync(IFormFile file)
        {
            CommonUtils.CheckSizeFile(file);
            CommonUtils.CheckFormatFile(file, FileTypes.IMAGE);
            var paths = await _filePath.GetPath(FOLDER_SERVICE, PathFolder.FOLDER_AVATAR, _session.TenantId);
            var subUrl = await _fileProvider.UploadFileAsync(paths, file);
            return subUrl;
        }
    }
}
