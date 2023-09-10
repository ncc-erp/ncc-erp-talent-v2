using System.Threading.Tasks;
using Abp.Application.Services;
using TalentV2.Sessions.Dto;

namespace TalentV2.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
