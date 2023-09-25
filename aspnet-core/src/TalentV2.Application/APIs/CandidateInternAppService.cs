using Abp.Authorization;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Requisitions;
using TalentV2.Entities;
using TalentV2.Notifications.Mail.Dtos;
using TalentV2.Utils;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class CandidateInternAppService : TalentV2AppServiceBase
    {
        private readonly ICandidateManager _candidateManager;
        private readonly IRequisitionManager _requisitionManager;
        public CandidateInternAppService(ICandidateManager candidateManager, IRequisitionManager requisitionManager) 
        {
            _candidateManager = candidateManager;
            _requisitionManager = requisitionManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewList)]
        public async Task<GridResult<CandidateDto>> GetAllPaging(CandidateFilterPaging paramFilters)
        {
            var query = _candidateManager
                .IQGetAllCVs()
                .Where(q => q.UserType == UserType.Intern)
                .WhereIf(paramFilters.RequestCVStatus.HasValue, q => q.RequisitionInfos.Any(s => s.RequestCVStatus == paramFilters.RequestCVStatus))
                .WhereIf(paramFilters.FromStatus.HasValue, q => q.HistoryChangeStatuses.Any(s => s.FromStatus == paramFilters.FromStatus))
                .WhereIf(paramFilters.ToStatus.HasValue, q => q.HistoryChangeStatuses.Any(s => s.ToStatus == paramFilters.ToStatus))
                .WhereIf(paramFilters.ProcessCVStatus.HasValue, q => q.ProcessCVStatus == paramFilters.ProcessCVStatus)
                .WhereIf(paramFilters.FromDate.HasValue, q => q.LastModifiedTime.Value.Date >= paramFilters.FromDate.Value.Date)
                .WhereIf(paramFilters.ToDate.HasValue, q => q.LastModifiedTime.Value.Date <= paramFilters.ToDate.Value.Date);

            if (paramFilters.SkillIds == null || paramFilters.SkillIds.IsEmpty())
            {
                return await query.GetGridResult(query, paramFilters);
            }
            if (paramFilters.SkillIds.Count() == 1 || !paramFilters.IsAndCondition)
            {
                var qcandidateIdsHaveAnySkill = _candidateManager
                    .IQGetCVHaveAnySkill(paramFilters.SkillIds)
                    .Distinct();
                query = from cv in query
                        join requestId in qcandidateIdsHaveAnySkill on cv.Id equals requestId
                        select cv;

                return await query.GetGridResult(query, paramFilters);
            }
            var CVIds = await _candidateManager.GetCVIdsHaveAllSkillAsync(paramFilters.SkillIds);
            query = query.Where(x => CVIds.Contains(x.Id));
            return await query.GetGridResult(query, paramFilters);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_Create)]
        public async Task<PersonBioDto> Create([FromForm] CreateCandidateDto param)
        {
            var cvId = await _candidateManager.CreateCV(param);

            return await _candidateManager.GetCVById(cvId);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_View)]
        public async Task<PersonBioDto> GetCVById(long cvId)
        {
            return await _candidateManager.GetCVById(cvId);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_Education_ViewList)]
        public async Task<List<EducationCandidateDto>> GetEducationCVsByCVId(long cvId)
        {
            return await _candidateManager.GetEducationCVsByCVId(cvId);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_ViewList)]
        public async Task<List<SkillCandidateDto>> GetSkillCVsByCVId(long cvId)
        {
            return await _candidateManager.GetSkillCVsByCVId(cvId);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV)]
        public async Task<CurrentRequisitionCandidateDto> GetCurrentRequisitionByCVId(long cvId)
        {
            return await _candidateManager.GetCurrentRequisitionByCVId(cvId);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_Edit)]
        public async Task<PersonBioDto> UpdateCV(UpdatePersonBioDto input)
        {
            return await _candidateManager.UpdateCV(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_Create)]
        public async Task<CurrentRequisitionCandidateDto> CreateRequestCV(CandidateRequestDto input)
        {
            var cvId = await _candidateManager.CreateRequestCV(input);
            if (cvId == default)
                throw new UserFriendlyException($"Request {input.RequestId} and CV {input.CvId} Already Existed");
            return await _candidateManager.GetCurrentRequisitionByCVId(cvId);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Create)]
        public async Task<SkillCandidateDto> CreateCVSkill(CreateUpdateSkillCandidateDto input)
        {
            return await _candidateManager.CreateCVSkill(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Edit)]
        public async Task<SkillCandidateDto> UpdateSkillCV(CreateUpdateSkillCandidateDto input)
        {
            return await _candidateManager.UpdateSkillCV(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Delete)]
        public async Task<IActionResult> DeleteSkillCV(long id)
        {
            await _candidateManager.DeleteSkillCV(id);
            return new OkObjectResult("Deleted Successfully");
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Create)]
        public async Task<EducationCandidateDto> CreateEducationCV(CreateUpdateEducationCandidateDto input)
        {
            return await _candidateManager.CreateEducationCV(input);
        }
        //[HttpPut]
        //[AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Edit)]
        //public async Task<EducationCandidateDto> UpdateEducationCV(CreateUpdateEducationCandidateDto input)
        //{
        //    return await _candidateManager.UpdateEducationCV(input);
        //}
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Delete)]
        public async Task<string> DeleteEducationCV(long id)
        {
            await _candidateManager.DeleteEducationCV(id);
            return "Deleted Successfully";
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewEditInterview)]
        public async Task<InterviewCandidateDto> AddInterviewerInCVRequest(CreateInterviewerCVRequestDto input)
        {
            return await _candidateManager.AddInterviewerInCVRequest(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditCapability)]
        public async Task<CapabilityCandidateDto> UpdateCapabilityCV(UpdateCapabilityCandidateDto input)
        {
            return await _candidateManager.UpdateCapabilityCV(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditCapability)]
        public async Task<List<CapabilityCandidateDto>> UpdateManyCapabilityCV(List<UpdateCapabilityCandidateDto> input)
        {
            return await _candidateManager.UpdateCapabilityCV(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditFactorCapabilityResult)]
        public async Task<List<CapabilityCandidateDto>> UpdateManyFactorsCapabilityCV(List<UpdateCapabilityCandidateDto> input)
        {
            return await _candidateManager.UpdateFactorsCapabilityCV(input);
        }
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationResult)]
        public async Task<ApplicationResultCandidateDto> UpdateApplicationResult(UpdateApplicationResultDto input)
        {
            return await _candidateManager.UpdateApplicationResult(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_Delete)]
        public async Task<string> Delete(long id)
        {
            var requestCVIds = await WorkScope.GetAll<RequestCV>()
                .Where(s => s.CVId == id)
                .Select(s => s.Id)
                .ToListAsync();
            foreach (var requestCVId in requestCVIds)
            {
                await _requisitionManager.DeleteRequestCV(requestCVId);
            }
            await _candidateManager.DeleteCV(id);
            return "Deleted Successfully";
        }
        [HttpGet]
        public async Task<List<IdAndNameDto>> GetAllUserCreated()
        {
            return await _candidateManager.GetUserCreated(UserType.Intern);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_Delete)]
        public async Task<string> DeleteRequestCV(long requestCvId)
        {
            await _requisitionManager.DeleteRequestCV(requestCvId);
            return "Deleted Successfully";
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewEditInterview)]
        public async Task<IActionResult> UpdateInterviewTime(UpdateInterviewTimeDto input)
        {
            await _candidateManager.UpdateInterviewTime(input);
            return new OkObjectResult("Updated Successfully");
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewEditInterview)]
        public async Task<string> DeleteRequestCVInterview(long id)
        {
            await _candidateManager.DeleteRequestCVInterview(id);
            return "Deleted Successfully";
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_Edit)]
        public async Task<string> UpdateFileAvatar([FromForm] UpdateFileAvatarDto input)
        {
            return await _candidateManager.UpdateAvatar(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_Edit)]
        public async Task<string> UpdateFileCV([FromForm] UpdateFileCVDto input)
        {
            return await _candidateManager.UpdateCV(input);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV)]
        public async Task<List<HistoryCandidateDto>> GetHistoryCV(long cvId)
        {
            return await _candidateManager.GetHistoryCV(cvId);
        }
        [HttpGet]
        public async Task<ValidCandidateDto> ValidEmail(string email, long? cvId)
        {
            return await _candidateManager.ValidEmail(email, cvId);
        }
        [HttpGet]
        public async Task<ValidCandidateDto> ValidPhone(string phone, long? cvId)
        {
            return await _candidateManager.ValidPhone(phone, cvId);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_SendMail)]
        public async Task<MailPreviewInfoDto> PreviewBeforeSendMailCV(long cvId)
        {
            return await _candidateManager.PreviewBeforeSendMailCV(cvId);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_SendMail)]
        public async Task<MailPreviewInfoDto> PreviewBeforeSendMailRequestCV(long requestCVId)
        {
            return await _candidateManager.PreviewBeforeSendMailRequestCV(requestCVId);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_SendMail)]
        public async Task<MailDetailDto> SendMailCV(long cvId, MailPreviewInfoDto message)
        {
            return await _candidateManager.SendMailCV(cvId, message);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_SendMail)]
        public async Task<MailDetailDto> SendMailRequestCV(long requestCVId, MailPreviewInfoDto message)
        {
            return await _candidateManager.SendMailRequestCV(requestCVId, message);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_CreateAccountLMS)]
        public async Task<string> CreateAccountStudent(long cvId, long requestCVId)
        {
            return await _candidateManager.CreateAccountStudent(cvId, requestCVId);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_EditNote)]
        public async Task<UpdateCandidateNoteDto> UpdateCandidateNote(UpdateCandidateNoteDto input)
        {
            return await _candidateManager.UpdateNote(input);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_Clone)]
        public async Task<CandidateDto> CloneCandidateByCVId(long cvId)
        {
            var newCvId = await _candidateManager.CloneCandidateByCvId(cvId);
            return await _candidateManager
                .IQGetAllCVs()
                .FirstOrDefaultAsync(q => q.Id == newCvId);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditInterviewLevel)]
        public async Task<InterviewLevelCandidateDto> UpdateInterviewLevel(UpdateInterviewLevelDto input)
        {
            return await _candidateManager.UpdateInterviewLevel(input);
        }
        [HttpPut]
        public async Task<InterviewedDto> UpdateInterviewed(UpdateInterviewedDto input)
        {
            return await _candidateManager.UpdateInterviewed(input);
        }
        [HttpPost]
        public async Task<IActionResult> ExportInfo(DomainServices.Candidates.Dtos.ExportInput input)
        {
            return await _candidateManager.ExportInfo(input);
        }
        [HttpPost]
        public async Task<IActionResult> ExportOnboard(DomainServices.Candidates.Dtos.ExportInput input)
        {
            return await _candidateManager.ExportOnboard(input);
        }
    }
}
