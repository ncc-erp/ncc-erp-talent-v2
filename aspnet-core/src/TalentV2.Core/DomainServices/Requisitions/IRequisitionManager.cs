using Abp.Domain.Entities;
using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServices.Requisitions.Dtos;

namespace TalentV2.DomainServices.Requisitions
{
    public interface IRequisitionManager : IDomainService
    {
        IQueryable<RequisitionDto> IQGetAllRequisition();
        Task<long> CreateRequisitonStaff(CreateRequisitionStaffDto input, long resourceRequestId = default);
        Task<long> CreateRequisitionIntern(CreateRequisitionInternDto input, long resourceRequestId = default);
        Task<RequisitionDto> GetRequisitionById(long id);
        Task<long> UpdateRequisitionStaff(UpdateRequisitionStaffDto input);
        Task<long> UpdateRequisitionIntern(UpdateRequisitionInternDto input);
        Task CloseRequestByRequestId(long requestId, bool isProjectTool = false);
        Task ReOpenRequestByRequestId(long requestId);
        Task Delete(long id);
        Task<List<CVRequisitionDto>> GetCVsByRequestId(long requestId);
        Task<long> CloneRequestByRequestId(long requestId, int quantity = default);
        Task DeleteRequestCV(long requestCvId);
        IQueryable<CVAvailableCloneDto> IQGetCVAvailableClone();
        IQueryable<long> IQGetRequestHaveAnySkill(List<long> skillIds);
        Task<List<long>> GetRequestIdsHaveAllSkillAsync(List<long> skillIds);
        Task<List<long>> GetCVIdsByRequestId(long requestId);
        Task<RequisitionToCloseAndCloneDto> GetRequisitionToCloseAndClone(long requestId);
        IQueryable<RequisitionToCloseAndCloneAllDto> IQGetAllCloseAndCloneRequisition();
        Task<CloneRequisitionDto> GetRequisitionByRequestId(long requestId, DateTime? timeNeed, string note,int quantity);

    }
}
