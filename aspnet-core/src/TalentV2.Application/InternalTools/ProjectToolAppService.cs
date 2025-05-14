using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.DomainServices.Requisitions;
using TalentV2.DomainServices.Requisitions.Dtos;
using TalentV2.Entities;
using TalentV2.InternalTools.Dtos;
using TalentV2.Ncc;
using TalentV2.Notifications.Komu;
using TalentV2.Notifications.Komu.Dtos;
using TalentV2.Notifications.MezonMessageTemplates;
using TalentV2.Notifications.Templates.Dtos;
using TalentV2.Utils;
using TalentV2.WebServices.ExternalServices.MezonWebhooks;

namespace TalentV2.InternalTools
{
    public class ProjectToolAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        private readonly IRequisitionManager _requisitionManager;
        private readonly MezonWebhookService _mezonWebhookService;
        public ProjectToolAppService(
            ICategoryManager categoryManager,
            IRequisitionManager requisitionManager,
            MezonWebhookService mezonWebhookService
        )
        {
            _categoryManager = categoryManager;
            _requisitionManager = requisitionManager;
            _mezonWebhookService = mezonWebhookService;
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
        [HttpPost]
        [NccAuth]
        public async Task<ProjectToolReponseDto> CreateRequestFromProject(CreateFromRequestProjectDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                if (await WorkScope.GetAll<Request>().AnyAsync(s => s.ProjectToolRequestId == input.ResourceRequestId))
                    return new ProjectToolReponseDto
                    {
                        Success = false,
                        Result = "The request is already created Id " + input.ResourceRequestId
                    };
                var skillIds = await WorkScope.GetAll<Skill>()
                .Where(q => input.SkillNames.Contains(q.Name))
                .Select(q => q.Id)
                .ToListAsync();
                long requestId;
                int type;
                string perPath = string.Empty;
                if (CommonUtils.ListLevelStaff.Any(s => s.Id == input.Level.GetHashCode()))
                {
                    var request = new CreateRequisitionStaffDto
                    {
                        Level = input.Level,
                        BranchId = input.BranchId,
                        Note = input.Note,
                        SubPositionId = input.SubPositionId,
                        Priority = input.Priority,
                        Quantity = input.Quantity,
                        SkillIds = skillIds,
                        TimeNeed = input.TimeNeed,
                        UserType = UserType.Staff
                    };
                    requestId = await _requisitionManager.CreateRequisitonStaff(request, input.ResourceRequestId);
                    type = UserType.Staff.GetHashCode();
                    perPath = "req-staff";
                }
                else
                {
                    var request = new CreateRequisitionInternDto
                    {
                        BranchId = input.BranchId,
                        Note = input.Note,
                        SubPositionId = input.SubPositionId,
                        Priority = input.Priority,
                        Quantity = input.Quantity,
                        SkillIds = skillIds,
                        TimeNeed = input.TimeNeed,
                        UserType = UserType.Intern
                    };
                    requestId = await _requisitionManager.CreateRequisitionIntern(request, input.ResourceRequestId);
                    type = UserType.Intern.GetHashCode();
                    perPath = "req-intern";
                }
                string appUrl = $"app/requisition/{perPath}/{requestId}?type={type}";

                var branchName = await WorkScope.GetAll<Branch>()
                    .Where(s => s.Id == input.BranchId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync();
                var subPosisitonName = await WorkScope.GetAll<SubPosition>()
                    .Where(s => s.Id == input.SubPositionId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync();

                var dataTemplate = new RequestFromProjectTemplate
                {
                    Note = input.Note,
                    UserType = (UserType)type,
                    RequestId = requestId,
                    BranchName = branchName,
                    SubPositionName = subPosisitonName,
                    URL = TalentConstants.BaseFEAddress + appUrl,
                    Level = input.Level,
                };

                if (dataTemplate.UserType == UserType.Staff)
                {
                    _mezonWebhookService.SendMessage(MezonMessageTemplate.RequestStaffFromProject(dataTemplate), MezonWebhookConstant.MessageFunction.RequestStaffFromProjectFunction);
                }
                else
                {
                    _mezonWebhookService.SendMessage(MezonMessageTemplate.RequestInternFromProject(dataTemplate), MezonWebhookConstant.MessageFunction.RequestInternFromProjectFunction);
                }

                return new ProjectToolReponseDto
                {
                    Success = true,
                    Result = appUrl
                };
            }
        }
        [HttpPost]
        [NccAuth]
        public async Task CancelRequest(CloseRequestByProjectToolDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var requestId = await WorkScope.GetAll<Request>()
                .Where(q => q.ProjectToolRequestId == input.ResourceRequestId)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
                await _requisitionManager.CloseRequestByRequestId(requestId, true);
            }
        }
    }
}
