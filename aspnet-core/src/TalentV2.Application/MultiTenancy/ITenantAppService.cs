using Abp.Application.Services;
using TalentV2.MultiTenancy.Dto;

namespace TalentV2.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

