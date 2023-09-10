using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Services;
using TalentV2.DomainServices.NccCVs.EmployeePositions.Dtos;

namespace TalentV2.DomainServices.NccCVs.EmployeePositions
{
    public interface IEmployeePositionManager : IDomainService
    {
        IQueryable<EmployeePositionDto> IQGetAll();
        Task<EmployeePositionDto> Create(EmployeePositionDto input);
        Task<EmployeePositionDto> Update(EmployeePositionDto input);
        Task Delete(long Id);
    }
}
