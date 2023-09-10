using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.FileServices.Paths
{
    public interface IFilePath
    {
        Task<List<string>> GetPath(string folderService, string folder, int? tenantId);
    }
}
