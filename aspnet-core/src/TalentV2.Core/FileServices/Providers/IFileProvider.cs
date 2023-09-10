using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Const;

namespace TalentV2.FileServices.Providers
{
    public interface IFileProvider
    {
        Task<string> UploadFileAsync(List<string> paths, IFormFile file);
    }
}
