using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories;
using TalentV2.DomainServices.Categories.Dtos;
//using TalentV2.DomainServices.Requisitions;
//using TalentV2.DomainServices.Requisitions.Dtos;
using TalentV2.Entities;
using TalentV2.InternalTools.Dtos;
using TalentV2.Ncc;
using TalentV2.Notifications.Komu;
using TalentV2.Notifications.Komu.Dtos;
using TalentV2.Utils;

namespace TalentV2.InternalTools
{
    public class ProjectToolAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        //private readonly IRequisitionManager _requisitionManager;
        private readonly IKomuNotification _komuNotification;
        public ProjectToolAppService(
            ICategoryManager categoryManager,
           // IRequisitionManager requisitionManager,
            IKomuNotification komuNotification
        )
        {
            _categoryManager = categoryManager;
            //_requisitionManager = requisitionManager;
            _komuNotification = komuNotification;
        }

        [HttpGet]
        [NccAuth]
        public async Task<List<DropdownPositionDto>> GetSubPositions()
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                return await WorkScope.GetAll<SubPosition>()
                            .GroupBy(s => new { s.PositionId, s.Position.Name })
                            .Select(gr => new DropdownPositionDto
                            {
                                Id = gr.Key.PositionId,
                                Position = gr.Key.Name,
                                Items = gr.Select(s => new DropdownSubPositionDto
                                {
                                    Id = s.Id,
                                    SubPosition = s.Name
                                }).ToList()
                            })
                            .OrderBy(s => s.Position)
                            .ToListAsync();
            }
        }

        [HttpGet]
        [NccAuth]
        public async Task<List<BranchDto>> GetBranches()
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                return await _categoryManager.IQGetAllBranches().ToListAsync();
            }
        }
        //[HttpPost]
        //[NccAuth]
        //public async Task<ProjectToolReponseDto> CreateRequestFromProject(CreateFromRequestProjectDto input)
        //{
        //    using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
        //    {
        //        if (await WorkScope.GetAll<Request>().AnyAsync(s => s.ProjectToolRequestId == input.ResourceRequestId))
        //            return new ProjectToolReponseDto
        //            {
        //                Success = false,
        //                Result = "The request is already created Id " + input.ResourceRequestId
        //            };
        //        var skillIds = await WorkScope.GetAll<Skill>()
        //        .Where(q => input.SkillNames.Contains(q.Name))
        //        .Select(q => q.Id)
        //        .ToListAsync();
        //        long requestId;
        //        int type;
        //        string perPath = string.Empty;
        //        if (CommonUtils.ListLevelStaff.Any(s => s.Id == input.Level.GetHashCode()))
        //        {
        //            var request = new CreateRequisitionStaffDto
        //            {
        //                Level = input.Level,
        //                BranchId = input.BranchId,
        //                Note = input.Note,
        //                SubPositionId = input.SubPositionId,
        //                Priority = input.Priority,
        //                Quantity = input.Quantity,
        //                SkillIds = skillIds,
        //                TimeNeed = input.TimeNeed,
        //                UserType = UserType.Staff
        //            };
        //            requestId = await _requisitionManager.CreateRequisitonStaff(request, input.ResourceRequestId);
        //            type = UserType.Staff.GetHashCode();
        //            perPath = "req-staff";
        //        }
        //        else
        //        {
        //            var request = new CreateRequisitionInternDto
        //            {
        //                BranchId = input.BranchId,
        //                Note = input.Note,
        //                SubPositionId = input.SubPositionId,
        //                Priority = input.Priority,
        //                Quantity = input.Quantity,
        //                SkillIds = skillIds,
        //                TimeNeed = input.TimeNeed,
        //                UserType = UserType.Intern
        //            };
        //            requestId = await _requisitionManager.CreateRequisitionIntern(request, input.ResourceRequestId);
        //            type = UserType.Intern.GetHashCode();
        //            perPath = "req-intern";
        //        }
        //        string uri = $"app/requisition/{perPath}/{requestId}?type={type}";
        //        await _komuNotification.NotifyRequestFromProject(new NotificationRequestFromProject
        //        {
        //            Note = input.Note,
        //            UserType = (UserType)type,
        //            RequestId = requestId,
        //            BranchId = input.BranchId,
        //            SubPositionId = input.SubPositionId,
        //            URI = uri,
        //            Level = input.Level,
        //        });

        //        return new ProjectToolReponseDto
        //        {
        //            Success = true,
        //            Result = uri
        //        };
        //    }
        //}
        //[HttpPost]
        //[NccAuth]
        //public async Task CancelRequest(CloseRequestByProjectToolDto input)
        //{
        //    using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
        //    {
        //        var requestId = await WorkScope.GetAll<Request>()
        //        .Where(q => q.ProjectToolRequestId == input.ResourceRequestId)
        //        .Select(s => s.Id)
        //        .FirstOrDefaultAsync();
        //        await _requisitionManager.CloseRequestByRequestId(requestId, true);
        //    }
        //}
    }
}
