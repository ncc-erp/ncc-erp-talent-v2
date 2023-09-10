using Abp.Dependency;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TalentV2.FileServices.Services.ExternalCVs
{
    public interface IFileExternalCVService : ITransientDependency
    {
        Task<string> UploadCV(IFormFile file);
        Task<string> UploadAvatar(IFormFile file);
    }
}
