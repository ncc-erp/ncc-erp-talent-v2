using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using Microsoft.AspNetCore.Http;
using TalentV2.Sessions.Dto;
using TalentV2.Utils;

namespace TalentV2.Sessions
{
    public class SessionAppService : TalentV2AppServiceBase, ISessionAppService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionAppService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
                output.User.AvatarPath = CommonUtils.FullFilePath(output.User.AvatarPath);
            }
            return output;
        }
    }
}
