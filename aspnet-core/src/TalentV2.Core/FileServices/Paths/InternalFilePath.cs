using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.MultiTenancy;

namespace TalentV2.FileServices.Paths
{
    public class InternalFilePath : IFilePath
    {
        private readonly TenantManager _tenantManager;
        public InternalFilePath(TenantManager tenantManager) 
        {
            _tenantManager = tenantManager;
        }
        public async Task<List<string>> GetPath(string folderService, string folder, int? tenantId)
        {
            var paths = new List<string>();
            var tenantFolderName = "tenant_" + await GetTenantName(tenantId);
            paths.Add(tenantFolderName);
            paths.Add(TalentConstants.PROJECT_NAME);
            paths.Add(folderService);
            paths.Add(folder);
            return paths;
        }
        private async Task<string> GetTenantName(int? tenantId)
        {
            if (tenantId.HasValue)
            {
                var tenant = await _tenantManager.GetByIdAsync(tenantId.Value);
                return tenant.TenancyName;
            }
            return "host";
        } 
    }
}
