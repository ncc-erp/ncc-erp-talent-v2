using Abp.Domain.Services;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.DomainServices.ApplyCVs.Dtos;

namespace TalentV2.DomainServices.ApplyCVs
{
    public interface IApplyCvManager : IDomainService
    {
        Task<long> Create(CreateApplyCVDto createApplyCVDto);
        Task<ApplyCVDto> GetApplyCVById(long applyCVId);
        IQueryable<ApplyCVDto> IQGetAll();
        Task<ApplyCVDto> GetCVById(long cvId);
    }
}
