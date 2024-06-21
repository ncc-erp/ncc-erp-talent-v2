using Abp.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.Notifications.Mail.Dtos;

namespace TalentV2.DomainServices.Candidates
{
    public interface ICandidateManager : IDomainService
    {
        Task<long> CreateCV(CreateCandidateDto input);
        //TO-DO: detail application history
        Task<PersonBioDto> GetCVById(long cvId);
        Task<List<EducationCandidateDto>> GetEducationCVsByCVId(long cvId);
        Task<List<SkillCandidateDto>> GetSkillCVsByCVId(long cvId);
        Task<List<CapabilityCandidateDto>> GetCapabilityCVsByRequestCVId(long requestCvId);
        Task<CurrentRequisitionCandidateDto> GetCurrentRequisitionByCVId(long cvId);
        Task<PersonBioDto> UpdateCV(UpdatePersonBioDto input);
        Task<long> CreateRequestCV(RequesitionCVDto input);
        Task<long> CreateCandidateRequestCV(CandidateRequestCVDto input);
        Task<SkillCandidateDto> CreateCVSkill(CreateUpdateSkillCandidateDto input);
        Task<SkillCandidateDto> UpdateSkillCV(CreateUpdateSkillCandidateDto input);
        Task DeleteSkillCV(long id);
        Task<EducationCandidateDto> CreateEducationCV(CreateUpdateEducationCandidateDto input);
        Task<EducationCandidateDto> UpdateEducationCV(CreateUpdateEducationCandidateDto input);
        Task DeleteEducationCV(long id);
        Task<InterviewCandidateDto> AddInterviewerInCVRequest(CreateInterviewerCVRequestDto input);
        Task<CapabilityCandidateDto> UpdateCapabilityCV(UpdateCapabilityCandidateDto input);
        Task<List<CapabilityCandidateDto>> UpdateCapabilityCV(List<UpdateCapabilityCandidateDto> input);
        Task<List<CapabilityCandidateDto>> UpdateFactorsCapabilityCV(List<UpdateCapabilityCandidateDto> input);
        Task<ApplicationResultCandidateDto> UpdateApplicationResult(UpdateApplicationResultDto input);
        Task<InterviewLevelCandidateDto> UpdateInterviewLevel(UpdateInterviewLevelDto input);
        Task<InterviewLevelCandidateDto> GetInterviewLevelByRequestCVId(long requestCvId);
        Task<ApplicationResultCandidateDto> GetApplicationResultByRequestCVId(long requestCvId);
        Task<InterviewedDto> UpdateInterviewed(UpdateInterviewedDto input);
        Task<List<InterviewCandidateDto>> GetInterviewerCVsByRequestCVId(long requestCvId);
        Task CreateRequestCVHistory(HistoryRequestCVDto input);
        Task CreateRequestCVStatusChangeHistory(StatusChangeRequestCVDto input);
        Task DeleteCV(long Id);
        IQueryable<InterviewCandidateDto> IQGetInterviewCVs();
        IQueryable<CandidateDto> IQGetAllCVs();
        IQueryable<long> IQGetCVHaveAnySkill(List<long> skillIds);
        Task<List<long>> GetCVIdsHaveAllSkillAsync(List<long> skillIds);
        Task<List<IdAndNameDto>> GetUserCreated(UserType userType);
        Task UpdateInterviewTime(UpdateInterviewTimeDto input);
        Task DeleteRequestCVInterview(long id);
        Task<string> UpdateAvatar(UpdateFileAvatarDto input);
        Task<string> UpdateCV(UpdateFileCVDto input);
        Task<List<HistoryCandidateDto>> GetHistoryCV(long cvId);
        Task<ValidCandidateDto> ValidEmail(string email, long? cvId);
        Task<ValidCandidateDto> ValidPhone(string phone, long? cvId);
        Task SetFailCV(long cvId);
        Task SetPassCV(IEnumerable<long> cvIds);
        Task<UpdateCandidateNoteDto> UpdateNote(UpdateCandidateNoteDto input);
        void UpdateHrmTempEmployee(long requestCVId);
        Task<long> CloneCandidateByCvId(long cvId);

        #region export Infomation

        Task<FileContentResult> ExportReport(ExportReport input);

        #endregion

        #region send mail CV
        Task<MailPreviewInfoDto> PreviewBeforeSendMailCV(long cvId);
        Task<MailPreviewInfoDto> PreviewBeforeSendMailRequestCV(long cvId);
        Task<MailDetailDto> SendMailCV(long cvId, MailPreviewInfoDto message);
        Task<MailDetailDto> SendMailRequestCV(long cvId, MailPreviewInfoDto message);
        Task<string> CreateAccountStudent(long cvId, long requestCVId, StatusCreateAccount statusCreateAccount);

        #endregion
    }
}
