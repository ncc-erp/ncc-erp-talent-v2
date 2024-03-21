using Abp.Domain.Entities;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.Entities;
using TalentV2.Notifications.Mail.Dtos;

namespace TalentV2.DomainServices.Candidates
{
    public interface ICandidateManager : IDomainService
    {
        Task<long> CreateCV(CreateCandidateDto input);
        //TO-DO: detail application history
        Task<List<EducationCandidateDto>> GetEducationCVsByCVId(long cvId);
        Task<List<SkillCandidateDto>> GetSkillCVsByCVId(long cvId);
        Task<SkillCandidateDto> CreateCVSkill(CreateUpdateSkillCandidateDto input);
        Task<SkillCandidateDto> UpdateSkillCV(CreateUpdateSkillCandidateDto input);
        Task DeleteCV(long Id);
        IQueryable<long> IQGetCVHaveAnySkill(List<long> skillIds);
        Task<List<long>> GetCVIdsHaveAllSkillAsync(List<long> skillIds);
        Task<List<IdAndNameDto>> GetUserCreated(UserType userType);
        Task<string> UpdateAvatar(UpdateFileAvatarDto input);
        Task<string> UpdateCV(UpdateFileCVDto input);
        Task<ValidCandidateDto> ValidEmail(string email, long? cvId);
        Task<ValidCandidateDto> ValidPhone(string phone, long? cvId);

        #region export Infomation


        #endregion

        #region send mail CV

        #endregion
    }
}
