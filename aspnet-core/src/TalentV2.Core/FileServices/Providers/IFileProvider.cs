using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TalentV2.FileServices.Providers
{
    public interface IFileProvider
    {
        Task<string> UploadFileAsync(List<string> paths, IFormFile file);

        Task<Stream> ReadFileAsync(List<string> paths, string fileName);

        Task<string> CopyFileAsync(List<string> sourcePaths, List<string> destinationPaths, string fileName, bool hasTimestamp = false);

        Task MoveFileAsync(List<string> sourcePaths, List<string> destinationPaths, string fileName, bool hasTimestamp = false);

        Task MoveCvFileToFolderAsync(List<string> sourcePaths, string fileName, string folderName, bool hasTimestamp = false);

        Task<List<string>> GetFileNamesAsync(List<string> paths);
    }
}