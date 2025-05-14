using Abp.Domain.Services;
using System.Threading.Tasks;
using TalentV2.DomainServices.MezonCVs.Dtos;

namespace TalentV2.DomainServices.MezonCVs
{
    public interface IMezonCVManager : IDomainService
    {
        Task<long> CreateMezonInternCV(CreateMezonInternCVDto input);
        Task<MezonInternCVDto> GetCVById(long id);
        Task<MezonCVFormDto> GetMezonCVFormData();
    }
}
