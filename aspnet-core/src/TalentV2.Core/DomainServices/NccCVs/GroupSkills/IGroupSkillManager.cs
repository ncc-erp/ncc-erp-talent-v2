using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Services;
using TalentV2.DomainServices.NccCVs.GroupSkills.Dtos;

namespace TalentV2.DomainServices.NccCVs.GroupSkills
{
    public interface IGroupSkillManager : IDomainService
    {
        IQueryable<GroupSkillDto> IQGetAll();
        Task<GroupSkillDto> Create(GroupSkillDto input);
        Task<GroupSkillDto> Update(GroupSkillDto input);
        Task Delete(long Id);
    }
}
