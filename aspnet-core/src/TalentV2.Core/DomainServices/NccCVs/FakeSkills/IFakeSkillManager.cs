using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Services;
using TalentV2.DomainServices.NccCVs.FakeSkills.Dtos;

namespace TalentV2.DomainServices.NccCVs.FakeSkills
{
    public interface IFakeSkillManager : IDomainService
    {
        IQueryable<FakeSkillDto> IQGetAll();
        Task<FakeSkillDto> Create(FakeSkillDto input);
        Task<FakeSkillDto> Update(FakeSkillDto input);
        Task Delete(long Id);
    }
}
