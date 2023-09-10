using Abp.Dependency;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.FileServices.Services.Employees
{
    public interface IFileEmployeeService : ITransientDependency
    {
        Task<string> UploadCV(IFormFile file);
        Task<string> UploadAvatar(IFormFile file);
    }
}
