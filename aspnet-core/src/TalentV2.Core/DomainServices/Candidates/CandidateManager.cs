using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Authorization.Roles;
using TalentV2.Configuration;
using TalentV2.Constants.Const;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.Entities;
using TalentV2.FileServices.Services.Candidates;
using TalentV2.Notifications.Komu;
using TalentV2.Notifications.Mail;
using TalentV2.Notifications.Mail.Dtos;
using TalentV2.Notifications.Templates;
using TalentV2.Utils;
using TalentV2.WebServices.InternalServices.HRM;
using TalentV2.WebServices.InternalServices.HRM.Dtos;
using TalentV2.WebServices.InternalServices.LMS;
using TalentV2.WebServices.InternalServices.LMS.Dtos;

namespace TalentV2.DomainServices.Candidates
{
    public class CandidateManager : BaseManager, ICandidateManager
    {
        private readonly IFileCandidateService _fileCandidate;

        private readonly IMailService _mailService;

        private readonly LMSService _lmsService;

        private readonly IKomuNotification _komuNotification;

        private readonly HRMService _hrmService;

        public CandidateManager(
            IFileCandidateService fileCandidate,
            IMailService mailService,
            LMSService lmsService,
            IKomuNotification komuNotification,
            HRMService hrmService
        )
        {
            _fileCandidate = fileCandidate;
            _mailService = mailService;
            _lmsService = lmsService;
            _komuNotification = komuNotification;
            _hrmService = hrmService;
        }

        public async Task<long> CreateCV(CreateCandidateDto input)
        {
            var cv = ObjectMapper.Map<CV>(input);
            cv.Phone = Utils.StringExtensions.ReplaceWhitespace(cv.Phone);
            await WorkScope.InsertAsync<CV>(cv);
            CommonUtils.CheckFormatFile(input.Avatar, FileTypes.IMAGE);
            CommonUtils.CheckFormatFile(input.CV, FileTypes.DOCUMENT);
            var applyCV = WorkScope.GetAll<ApplyCV>()
                              .FirstOrDefault(x => x.Id == input.ApplyId);

            if (input.Avatar != null)
            {
                string avatarLink = await _fileCandidate.UploadAvatar(input.Avatar);
                cv.Avatar = avatarLink;
            }
            else
            {
                cv.Avatar = input.AvatarCv;
            }

            if (input.CV != null)
            {
                string cvLink = await _fileCandidate.UploadCV(input.CV);
                cv.LinkCV = cvLink;
            }

            if (applyCV != null)
            {
                applyCV.Applied = true;
                await WorkScope.UpdateAsync(applyCV);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return cv.Id;
        }


        public async Task<List<EducationCandidateDto>> GetEducationCVsByCVId(long cvId)
        {
            var educationCandidate = await IQGetEducationCVs()
                .Where(q => q.CVId == cvId)
                .ToListAsync();
            return educationCandidate;
        }

        public IQueryable<EducationCandidateDto> IQGetEducationCVs()
        {
            var query = WorkScope.GetAll<CVEducation>()
               .Select(s => new EducationCandidateDto
               {
                   Id = s.Id,
                   CVId = s.CVId,
                   EducationId = s.EducationId,
                   EducationName = s.Education.Name,
                   EducationTypeId = s.Education.EducationTypeId,
                   EducationTypeName = s.Education.EducationType.Name,
                   CreationTime = s.CreationTime,
                   CreatorName = s.CreatorUserId.HasValue ?
                                    s.CreatorUser.FullName :
                                    "",
                   LastModifiedTime = s.LastModificationTime,
                   LastModifiedName = s.LastModifierUserId.HasValue ?
                                        s.LastModifierUser.FullName :
                                        ""
               });
            return query;
        }

        public async Task<List<SkillCandidateDto>> GetSkillCVsByCVId(long cvId)
        {
            var skillCandidate = await IQGetSkillCVs()
                .Where(q => q.CVId == cvId)
                .ToListAsync();
            return skillCandidate;
        }

        public IQueryable<SkillCandidateDto> IQGetSkillCVs()
        {
            var query = WorkScope.GetAll<CVSkill>()
                .Select(s => new SkillCandidateDto
                {
                    Id = s.Id,
                    CVId = s.CVId,
                    SkillId = s.SkillId,
                    LevelSkill = s.Level,
                    Note = s.Note,
                    SkillName = s.Skill.Name,
                    CreationTime = s.CreationTime,
                    CreatorName = s.CreatorUserId.HasValue ?
                                    s.CreatorUser.FullName :
                                    "",
                    LastModifiedTime = s.LastModificationTime,
                    LastModifiedName = s.LastModifierUserId.HasValue ?
                                        s.LastModifierUser.FullName :
                                        ""
                });
            return query;
        }


        public async Task<SkillCandidateDto> CreateCVSkill(CreateUpdateSkillCandidateDto input)
        {
            var allCvSkill = IQGetSkillCVs()
                .Where(q => q.CVId == input.CVId);
            if (await allCvSkill.AnyAsync(q => q.SkillId == input.SkillId))
                throw new UserFriendlyException("Skill Already Exists!");

            var CVSkill = ObjectMapper.Map<CVSkill>(input);

            var id = await WorkScope.InsertAndGetIdAsync<CVSkill>(CVSkill);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetSkillCVs()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<SkillCandidateDto> UpdateSkillCV(CreateUpdateSkillCandidateDto input)
        {
            var allCvSkill = IQGetSkillCVs()
                .Where(q => q.CVId == input.CVId);
            if (await allCvSkill.AnyAsync(q => q.SkillId == input.SkillId && q.Id != input.Id))
                throw new UserFriendlyException("Skill Already Exists!");

            var CVSkill = ObjectMapper.Map<CVSkill>(input);
            await WorkScope.UpdateAsync<CVSkill>(CVSkill);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetSkillCVs()
                .Where(q => q.Id == input.Id)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteSkillCV(long id)
        {
            await WorkScope.DeleteAsync<CVSkill>(id);
        }

        public async Task<EducationCandidateDto> CreateEducationCV(CreateUpdateEducationCandidateDto input)
        {
            var allCvEducation = IQGetEducationCVs()
                .Where(q => q.CVId == input.CVId);
            if (await allCvEducation.AnyAsync(q => q.EducationId == input.EducationId))
                throw new UserFriendlyException("Education Already Exists!");

            var CvEducation = ObjectMapper.Map<CVEducation>(input);
            var id = await WorkScope.InsertAndGetIdAsync(CvEducation);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetEducationCVs()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<EducationCandidateDto> UpdateEducationCV(CreateUpdateEducationCandidateDto input)
        {
            var allCvEducation = IQGetEducationCVs()
                .Where(q => q.CVId == input.CVId);
            if (await allCvEducation.AnyAsync(q => q.EducationId == input.EducationId && q.Id != input.Id))
                throw new UserFriendlyException("Education Already Exists!");

            var CvEducation = ObjectMapper.Map<CVEducation>(input);
            await WorkScope.UpdateAsync(CvEducation);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetEducationCVs()
                .Where(q => q.Id == input.Id)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteEducationCV(long id)
        {
            await WorkScope.DeleteAsync<CVEducation>(id);
        }

        private bool UserHasRole(long userId, string roleName)
        {
            var quser =
                    from ur in WorkScope.GetAll<UserRole, long>().Where(u => u.UserId == userId)
                    join role in WorkScope.GetAll<Role, int>().Where(s => s.NormalizedName == roleName.ToUpper())
                    on ur.RoleId equals role.Id into roles
                    from r in roles
                    select r.Id;
            return quser.Any();
        }


        public void UpdateHrmTempEmployee(long requestCVId)
        {
            var candidateInfo = WorkScope.GetAll<RequestCV>()
                .Where(x => x.Id == requestCVId)
                .Select(x => new UpdateHRMTempEmployeeDto
                {
                    FullName = x.CV.Name,
                    NCCEmail = x.CV.NCCEmail,
                    Phone = x.CV.Phone,
                    IsFemale = x.CV.IsFemale,
                    Status = x.Status,
                    UserType = x.CV.UserType,
                    FinalLevel = x.FinalLevel,
                    Salary = x.Salary,
                    OnboardDate = x.OnboardDate,
                    PersonalEmail = x.CV.Email,
                    SkillNames = x.Request.RequestSkills.Select(x => x.Skill.Name).ToList(),
                    PositionName = x.CV.SubPosition.Position.Name,
                    BranchName = x.CV.Branch.Name,
                    DateOfBirth = x.CV.Birthday
                }).FirstOrDefault();

            _hrmService.UpdateHRMTempEmployee(candidateInfo);
        }


        public async Task DeleteCV(long Id)
        {
            await WorkScope.DeleteAsync<CV>(Id);
        }

        public IQueryable<long> IQGetCVHaveAnySkill(List<long> skillIds)
        {
            if (skillIds == null || skillIds.IsEmpty())
            {
                throw new Exception("skillIds null or empty");
            }
            return WorkScope.GetAll<CVSkill>()
                   .Where(s => skillIds.Contains(s.SkillId))
                   .Select(s => s.CVId);
        }

        public async Task<List<long>> GetCVIdsHaveAllSkillAsync(List<long> skillIds)
        {
            if (skillIds == null || skillIds.IsEmpty())
            {
                throw new Exception("skillIds null or empty");
            }

            var result = await WorkScope.GetAll<CVSkill>()
                    .Where(s => skillIds[0] == s.SkillId)
                    .Select(s => s.CVId)
                    .Distinct()
                    .ToListAsync();

            if (result == null || result.IsEmpty())
            {
                return new List<long>();
            }

            for (var i = 1; i < skillIds.Count(); i++)
            {
                var cvIds = await WorkScope.GetAll<CVSkill>()
                    .Where(s => skillIds[i] == s.SkillId)
                    .Select(s => s.CVId)
                    .Distinct()
                    .ToListAsync();

                result = result.Intersect(cvIds).ToList();

                if (result == null || result.IsEmpty())
                {
                    return new List<long>();
                }
            }
            return result;
        }

        public async Task<List<IdAndNameDto>> GetUserCreated(UserType userType)
        {
            var query = from cv in WorkScope.GetAll<CV>()
                        where cv.CreatorUserId.HasValue && cv.UserType == userType && !cv.CreatorUser.IsDeleted
                        select new IdAndNameDto
                        {
                            Id = cv.CreatorUserId.Value,
                            Name = cv.CreatorUserId == AbpSession.UserId ?
                                    "@Me" :
                                    cv.CreatorUser.FullName,
                        };
            var allCreator = query
                .Distinct()
                .AsNoTracking()
                .AsEnumerable()
                .OrderBy(x => x.Name);

            return allCreator.ToList();
        }


        public async Task DeleteRequestCVInterview(long id)
        {
            await WorkScope.DeleteAsync<RequestCVInterview>(id);
        }

        public async Task<string> UpdateAvatar(UpdateFileAvatarDto input)
        {
            var cv = await WorkScope.GetAsync<CV>(input.CVId);
            if (input.FileUpdate == null) return string.Empty;
            var subLink = await _fileCandidate.UploadAvatar(input.FileUpdate);
            cv.Avatar = subLink;
            await CurrentUnitOfWork.SaveChangesAsync();
            return CommonUtils.FullFilePath(subLink);
        }

        public async Task<string> UpdateCV(UpdateFileCVDto input)
        {
            var cv = await WorkScope.GetAsync<CV>(input.CVId);
            if (input.FileUpdate == null) return string.Empty;
            var subLink = await _fileCandidate.UploadCV(input.FileUpdate);
            cv.LinkCV = subLink;
            await CurrentUnitOfWork.SaveChangesAsync();
            return CommonUtils.FullFilePath(subLink);
        }

        public async Task<ValidCandidateDto> ValidEmail(string email, long? cvId)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            else
            {
                var cv = await WorkScope.GetAll<CV>()
                    .FirstOrDefaultAsync(s => s.Email == email && (cvId.HasValue ? cvId != s.Id : true));
                var currentCloneCV = await WorkScope.GetAll<CV>()
                .FirstOrDefaultAsync(s => cvId.HasValue && cvId == s.Id && s.isClone == true);
                if (currentCloneCV != null || cv == null)
                {
                    return null;
                }
                return new ValidCandidateDto
                {
                    CVId = cv.Id,
                    UserType = cv.UserType
                };
            }
        }

        public async Task<ValidCandidateDto> ValidPhone(string phone, long? cvId)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return null;
            }
            else
            {
                var cv = await WorkScope.GetAll<CV>()
                    .FirstOrDefaultAsync(s => s.Phone == phone && (cvId.HasValue ? cvId != s.Id : true));
                var currentCloneCV = await WorkScope.GetAll<CV>()
                .FirstOrDefaultAsync(s => cvId.HasValue && cvId == s.Id && s.isClone == true);
                if (currentCloneCV != null || cv == null)
                {
                    return null;
                }
                return new ValidCandidateDto
                {
                    CVId = cv.Id,
                    UserType = cv.UserType
                };
            }
        }

        public async Task SetFailCV(long cvId)
        {
            var cv = await WorkScope.GetAsync<CV>(cvId);
            cv.CVStatus = CVStatus.Failed;
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task SetPassCV(IEnumerable<long> cvIds)
        {
            (
                await WorkScope.GetAll<CV>()
                .Where(q => cvIds.Contains(q.Id))
                .ToListAsync()
            ).ForEach(cv =>
            {
                cv.CVStatus = CVStatus.Passed;
            });
            await CurrentUnitOfWork.SaveChangesAsync();
        }



        #region export infomation

        public byte[] CombineExcelFiles(byte[] excelBytes1, byte[] excelBytes2)
        {
            using (var package1 = new ExcelPackage(new MemoryStream(excelBytes1)))
            using (var package2 = new ExcelPackage(new MemoryStream(excelBytes2)))
            {
                var sheetsFromPackage2 = package2.Workbook.Worksheets.ToList();
                var combinedPackage = new ExcelPackage();
                foreach (var sheet in package1.Workbook.Worksheets)
                {
                    var newSheet = combinedPackage.Workbook.Worksheets.Add(sheet.Name, sheet);
                }
                foreach (var sheet in sheetsFromPackage2)
                {
                    var newSheet = combinedPackage.Workbook.Worksheets.Add(sheet.Name, sheet);
                }
                return combinedPackage.GetAsByteArray();
            }
        }

        #endregion

        #region send mail CV

        public async Task<MailPreviewInfoDto> PreviewBeforeSendMailCV(long cvId)
        {
            return await _mailService.GetContentMailCV(cvId);
        }

        public async Task<MailPreviewInfoDto> PreviewBeforeSendMailRequestCV(long requestCVId)
        {
            return await _mailService.GetContentMailRequestCV(requestCVId);
        }


        private async Task<ControlSendMail> CheckAllowSendMailFailedCV(long cvId)
        {
            var cvStatus = await WorkScope.GetAll<CV>()
                .Where(q => q.Id == cvId)
                .Select(s => s.CVStatus)
                .FirstOrDefaultAsync();

            if (cvStatus != CVStatus.Failed)
                return ControlSendMail.DENY;

            if (await _mailService.IsSentMailCV(cvId, MailFuncEnum.FailedCV))
                return ControlSendMail.WARN;

            return ControlSendMail.ALLOW;
        }


        public async Task<long> CloneCandidateByCvId(long cvId)
        {
            var oldCv = await WorkScope.GetAll<CV>()
                .Where(q => q.Id == cvId)
                .FirstOrDefaultAsync();
            var cvSkills = await WorkScope.GetAll<CVSkill>()
                .Where(q => q.CVId == cvId)
                .Select(q => new { q.SkillId, q.Note, q.Level, q.Skill })
                .ToListAsync();
            var cvEducations = await WorkScope.GetAll<CVEducation>()
                .Where(q => q.CVId == cvId)
                .Select(q => q.EducationId)
                .ToListAsync();

            oldCv.Id = 0;

            if (!cvSkills.IsEmpty())
            {
                var newCvSkills = new List<CVSkill>();
                cvSkills.ForEach(skill =>
                {
                    newCvSkills.Add(new CVSkill
                    {
                        CV = oldCv,
                        SkillId = skill.SkillId,
                        Note = skill.Note,
                        Level = skill.Level,
                    });
                });
                oldCv.CVSkills = newCvSkills;
            }
            if (!cvEducations.IsEmpty())
            {
                var newCVEducations = new List<CVEducation>();
                cvEducations.ForEach(educationId =>
                {
                    newCVEducations.Add(new CVEducation
                    {
                        CV = oldCv,
                        EducationId = educationId,
                    });
                });
                oldCv.CVEducations = newCVEducations;
            }
            oldCv.CreationTime = DateTimeUtils.GetNow();
            oldCv.CreatorUserId = null;
            oldCv.DeleterUserId = null;
            var newCv = await WorkScope.InsertAsync(oldCv);
            newCv.CVStatus = CVStatus.New;
            newCv.isClone = true;
            await CurrentUnitOfWork.SaveChangesAsync();
            return newCv.Id;
        }
        #endregion send mail CV
    }
}