using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.FileServices.Paths;
using TalentV2.FileServices.Providers;
using TalentV2.Utils;

namespace TalentV2.FileServices.Services.ApplyCV
{
    public class FileApplyCVService : IFileApplyCVService
    {
        private readonly string FOLDER_SERVICE = "applyCV";
        private readonly IAbpSession _session;
        private readonly IFileProvider _fileService;
        private readonly IFilePath _filePath;
        public FileApplyCVService(IAbpSession session, IFileProvider fileService, IFilePath filePath)
        {
            _session = session;
            _fileService = fileService;
            _filePath = filePath;
        }
        public async Task<string> UploadAttachCV(IFormFile file)
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
    }
}
