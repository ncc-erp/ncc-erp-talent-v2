using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using TalentV2.Constants.Dictionary;
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
        private readonly IConfiguration _configuration;

        public CandidateManager(
            IFileCandidateService fileCandidate,
            IMailService mailService,
            LMSService lmsService,
            IKomuNotification komuNotification,
            HRMService hrmService,
            IConfiguration configuration
        )
        {
            _fileCandidate = fileCandidate;
            _mailService = mailService;
            _lmsService = lmsService;
            _komuNotification = komuNotification;
            _hrmService = hrmService;
            _configuration = configuration;
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

        public async Task<PersonBioDto> GetCVById(long cvId)
        {
            var persionBio = await WorkScope.GetAll<CV>()
                .Where(q => q.Id == cvId)
                .Select(s => new PersonBioDto
                {
                    Id = s.Id,
                    Address = s.Address,
                    PathAvatar = s.Avatar,
                    PathLinkCV = s.LinkCV,
                    CVSourceId = s.CVSourceId,
                    DOB = s.Birthday,
                    Email = s.Email,
                    FullName = s.Name,
                    IsFemale = s.IsFemale,
                    Note = s.Note,
                    Phone = s.Phone,
                    SubPositionId = s.SubPositionId,
                    SubPositionName = s.SubPosition.Name,
                    BranchId = s.BranchId,
                    BranchName = s.Branch.Name,
                    ReferenceId = s.ReferenceId,
                    UserType = s.UserType,
                    CvStatus = s.CVStatus,
                    isClone = s.isClone,
                    CreatorUserId = s.CreatorUserId,
                    CreatorName = s.CreatorUser.Name,
                })
                .FirstOrDefaultAsync();
            var controlSendMail = await CheckAllowSendMail(cvId: cvId);
            persionBio.MailDetail = new MailDetailDto
            {
                IsAllowSendMail = controlSendMail == ControlSendMail.DENY ? false : true,
                IsSentMailStatus = controlSendMail == ControlSendMail.WARN ? true : false,
                MailStatusHistories = await _mailService.GetMailStatusHistoryByCVId(cvId)
            };
            return persionBio;
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

        public async Task<List<CapabilityCandidateDto>> GetCapabilityCVsByRequestCVId(long requestCvId)
        {
            return await IQGetCapability()
                .Where(q => q.RequestCvId == requestCvId).OrderBy(x => x.CapabilityId)
                .ToListAsync();
        }

        public async Task<CurrentRequisitionCandidateDto> GetCurrentRequisitionByCVId(long cvId)
        {
            var currentRequisition = await WorkScope.GetAll<RequestCV>()
            .Where(q => q.CVId == cvId)
            .Select(s => new CurrentRequisitionCandidateDto
            {
                Id = s.Id,
                CVName = s.CV.Name,
                CurrentRequisition = new CurrentRequisitionDto
                {
                    Id = s.RequestId,
                    BranchId = s.Request.BranchId,
                    BranchName = s.Request.Branch.Name,
                    DisplayBranchName = s.Request.Branch.DisplayName,
                    Level = s.Request.Level,
                    SubPositionId = s.Request.SubPositionId,
                    SubPositionName = s.Request.SubPosition.Name,
                    Priority = s.Request.Priority,
                    Note = s.Request.Note,
                    Quantity = s.Request.Quantity,
                    RequestStatus = s.Request.Status,
                    UserType = s.Request.UserType,
                    TimeNeed = s.Request.TimeNeed,
                    ProjectToolRequestId = s.Request.ProjectToolRequestId,
                    Skills = s.Request.RequestSkills.Select(x => new CategoryDto
                    {
                        Id = x.Id,
                        Name = x.Skill.Name
                    }).ToList(),
                    CreationTime = s.Request.CreationTime,
                    CreatorName = s.Request.CreatorUserId.HasValue ?
                                    s.Request.CreatorUser.FullName :
                                    "",
                    LastModifiedTime = s.Request.LastModificationTime,
                    LastModifiedName = s.Request.LastModifierUserId.HasValue ?
                                        s.Request.LastModifierUser.FullName :
                                        ""
                },
                InterviewTime = s.InterviewTime,
                CreationTime = s.CreationTime,
            })
            .OrderByDescending(x => x.CreationTime)
            .FirstOrDefaultAsync();
            if (currentRequisition == null)
                return null;
            currentRequisition.CapabilityCandidate = await GetCapabilityCVsByRequestCVId(currentRequisition.Id);
            currentRequisition.InterviewCandidate = await GetInterviewerCVsByRequestCVId(currentRequisition.Id);
            currentRequisition.ApplicationResult = await GetApplicationResultByRequestCVId(currentRequisition.Id);
            currentRequisition.InterviewLevel = await GetInterviewLevelByRequestCVId(currentRequisition.Id);
            currentRequisition.Interviewed = await GetInterviewedByRequestCVId(currentRequisition.Id);
            return currentRequisition;
        }

        public async Task<PersonBioDto> UpdateCV(UpdatePersonBioDto input)
        {
            var personBio = await WorkScope.GetAsync<CV>(input.Id);
            var requestCVs = await WorkScope.GetAll<RequestCV>()
            .FirstOrDefaultAsync(r => r.CVId == input.Id);
         
            var isSendNotification = personBio.UserType != input.UserType
                || personBio.BranchId != input.BranchId
                || (!string.IsNullOrEmpty(personBio.Phone) && !personBio.Phone.Equals(input.Phone))
                || (!string.IsNullOrEmpty(personBio.Email) && !personBio.Email.Equals(input.Email));
            if (input?.Note != requestCVs?.HRNote && requestCVs?.HRNote != null)
            {
                requestCVs.HRNote = input.Note;

            }
            ObjectMapper.Map<UpdatePersonBioDto, CV>(input, personBio);
            personBio.Phone = Utils.StringExtensions.ReplaceWhitespace(personBio.Phone);
            await CurrentUnitOfWork.SaveChangesAsync();

            if (isSendNotification)
            {
                var requestCV = await WorkScope.GetAll<RequestCV>()
                    .FirstOrDefaultAsync(r => r.CVId == input.Id);
                if (requestCV != null && requestCV.Status == RequestCVStatus.AcceptedOffer)
                    await _komuNotification.NotifyUpdatedPersonalInfoTemplate(requestCV.Id);
            }

            return await GetCVById(personBio.Id);
        }

        public async Task<long> CreateCandidateRequestCV(CandidateRequestCVDto input)
        {
            var request = await WorkScope.GetAll<Request>()
                .Where(q => q.Id == input.RequestId)
                .Select(s => new
                {
                    s.UserType,
                    s.SubPositionId
                })
                .FirstOrDefaultAsync();
            var cv = await WorkScope.GetAsync<CV>(input.CvId);
            var requestCVStatus = input.RequestCVStatus.HasValue ? input.RequestCVStatus.Value : RequestCVStatus.AddedCV;
            var requestCv = new RequestCV
            {
                RequestId = input.RequestId,
                CVId = input.CvId,
                Status = requestCVStatus,
                HRNote = cv.Note,
            };
            var requestCvId = await WorkScope.InsertAndGetIdAsync(requestCv);

            await CurrentUnitOfWork.SaveChangesAsync();

            await AddRequestCVCapabilityResult(requestCvId, request.UserType, request.SubPositionId);

            await CreateRequestCVHistory(new HistoryRequestCVDto
            {
                Id = requestCv.Id,
                Status = requestCVStatus
            });
            await CreateRequestCVStatusChangeHistory(new StatusChangeRequestCVDto
            {
                Id = requestCv.Id,
                FromStatus = null,
                ToStatus = requestCVStatus,
            });

            return input.CvId;
        }
        public async Task<long> CreateRequestCV(RequesitionCVDto input)
        {
            if (await WorkScope.GetAll<RequestCV>().AnyAsync(q => q.RequestId == input.RequestId && q.CVId == input.CvId))
                return default;

            var request = await WorkScope.GetAll<Request>()
             .Where(q => q.Id == input.RequestId)
             .Select(s => new
             {
                 s.UserType,
                 s.SubPositionId
             })
             .FirstOrDefaultAsync();

            var currentRequestCV = await WorkScope.GetAll<RequestCV>()
            .Where(q => q.RequestId == input.CurrentRequestId && q.CVId == input.CvId)
            .Select(s => new
            {
                s.Id,
                s.Status,
                s.Interviewed,
                s.InterviewLevel,
                s.ApplyLevel,
                s.Salary,
                s.FinalLevel,
                s.InterviewTime,
                s.HRNote,
                s.OnboardDate,
                s.EmailSent,
                s.LMSInfo,
                s.Percentage,
            })
           .FirstOrDefaultAsync();

            var cv = await WorkScope.GetAsync<CV>(input.CvId);
            var requestCVStatus = input.RequestCVStatus.HasValue ? input.RequestCVStatus.Value : RequestCVStatus.AddedCV;
            var requestCv = new RequestCV
            {
                RequestId = input.RequestId,
                CVId = input.CvId,
                Status = requestCVStatus
            };
            if (input.IsPresentForHr == true)
            {
                requestCv.Status = currentRequestCV.Status;
                requestCv.InterviewTime = currentRequestCV.InterviewTime;
                requestCv.ApplyLevel = currentRequestCV.ApplyLevel;
                requestCv.InterviewLevel = currentRequestCV.InterviewLevel;
                requestCv.FinalLevel = currentRequestCV.FinalLevel;
                requestCv.HRNote = currentRequestCV.HRNote;
                requestCv.OnboardDate = currentRequestCV.OnboardDate;
                requestCv.Salary = currentRequestCV.Salary;
                requestCv.EmailSent = currentRequestCV.EmailSent;
                requestCv.LMSInfo = currentRequestCV.LMSInfo;
                requestCv.Percentage = currentRequestCV.Percentage;
            }
            var requestCvId = await WorkScope.InsertAndGetIdAsync(requestCv);

            await CurrentUnitOfWork.SaveChangesAsync();
            if (currentRequestCV != null)
            {
                var currentRequestCVInterview = await WorkScope.GetAll<RequestCVInterview>()
                   .Where(q => q.RequestCVId == currentRequestCV.Id)
                   .ToListAsync();

                var currentRequestCVStatusHistories = await WorkScope.GetAll<RequestCVStatusHistory>()
                    .Where(q => q.RequestCVId == currentRequestCV.Id)
                    .ToListAsync();

                var currentRequestCVStatusChangeHistory = await WorkScope.GetAll<RequestCVStatusChangeHistory>()
                    .Where(q => q.RequestCVId == currentRequestCV.Id)
                    .ToListAsync();

                var cvCapabilityResult = await WorkScope.GetAll<RequestCVCapabilityResult>()
                    .Where(q => q.RequestCVId == currentRequestCV.Id)
                    .Select(q => new CVCapabilityResultDto{
                        CapabilityId = q.CapabilityId,
                        Score = q.Score,
                        Note = q.Note,
                    }).ToListAsync();
                await KeepRequestCVCapabilityResults(requestCvId, request.UserType, request.SubPositionId, cvCapabilityResult);

                foreach (var requestCVInterview in currentRequestCVInterview)
                {
                   await AddInterviewerInCVRequest(new CreateInterviewerCVRequestDto
                   {
                       InterviewerId = requestCVInterview.InterviewId,
                       RequestCvId = requestCvId,
                   });
                } 

                foreach (var requestCVStatusHistories in currentRequestCVStatusHistories)
                {
                  await CreateRequestCVHistory(new HistoryRequestCVDto
                    {
                    Id = requestCvId,
                    Status = requestCVStatusHistories.Status,
                  });
                }
                foreach (var requestCVStatusChangeHistory in currentRequestCVStatusChangeHistory)
                {
                    await CreateRequestCVStatusChangeHistory(new StatusChangeRequestCVDto
                    {
                        Id = requestCvId,
                        FromStatus = requestCVStatusChangeHistory.FromStatus,
                        ToStatus = requestCVStatusChangeHistory.ToStatus,
                    });
                }

                return input.CvId;
            } 
            await AddRequestCVCapabilityResult(requestCvId, request.UserType, request.SubPositionId);
   
            await CreateRequestCVHistory(new HistoryRequestCVDto
            {
                Id = requestCv.Id,
                Status = requestCVStatus
            });
            await CreateRequestCVStatusChangeHistory(new StatusChangeRequestCVDto
            {
                Id = requestCv.Id,
                FromStatus = null,
                ToStatus = requestCVStatus,
            });

            return input.CvId;
        }

        private async Task KeepRequestCVCapabilityResults(long requestCvId, UserType userType, long subPositionId, List<CVCapabilityResultDto> cvCapabilityResult)
        {
            var capabilitySettings = await WorkScope.GetAll<CapabilitySetting>()
                .Where(q => q.UserType == userType && q.SubPositionId == subPositionId && q.IsDeleted == false)
                .Select(s => new
                {
                    s.CapabilityId,
                    s.Factor
                })
                .ToListAsync();
            var capabilityResults = new List<RequestCVCapabilityResult>();
            foreach (var item in capabilitySettings)
            {
                var capabilityResult = new RequestCVCapabilityResult
                {
                    RequestCVId = requestCvId,
                    CapabilityId = item.CapabilityId,
                    Factor = item.Factor
                };
                foreach (var capability in cvCapabilityResult)
                {
                    if (capability.CapabilityId == item.CapabilityId)
                    {
                        capabilityResult.Score = capability.Score;
                        capabilityResult.Note = capability.Note;
                    }
                }
                capabilityResults.Add(capabilityResult);
            }
            await WorkScope.InsertRangeAsync(capabilityResults);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private async Task AddRequestCVCapabilityResult(long requestCvId, UserType userType, long subPositionId)
        {
            var capabilitySettings = await WorkScope.GetAll<CapabilitySetting>()
                .Where(q => q.UserType == userType && q.SubPositionId == subPositionId && q.IsDeleted == false)
                .Select(s => new
                {
                    s.CapabilityId,
                    s.Factor
                })
                .ToListAsync();
            foreach (var item in capabilitySettings)
            {
                var capabilityResult = new RequestCVCapabilityResult
                {
                    RequestCVId = requestCvId,
                    CapabilityId = item.CapabilityId,
                    Factor = item.Factor
                };
                await WorkScope.InsertAsync(capabilityResult);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
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

        public async Task<InterviewCandidateDto> AddInterviewerInCVRequest(CreateInterviewerCVRequestDto input)
        {
            var allInterviewersInCVRequest = await GetInterviewerCVsByRequestCVId(input.RequestCvId);

            if (allInterviewersInCVRequest.Any(s => s.InterviewId == input.InterviewerId))
                throw new UserFriendlyException("Interviewer Already Exists");

            var requestCvInterview = new RequestCVInterview
            {
                InterviewId = input.InterviewerId,
                RequestCVId = input.RequestCvId,
            };
            await WorkScope.InsertAsync<RequestCVInterview>(requestCvInterview);
            await CurrentUnitOfWork.SaveChangesAsync();
            AddRoleInterviewerToUser(input.InterviewerId);
            return await IQGetInterviewCVs()
                .Where(q => q.Id == requestCvInterview.Id)
                .FirstOrDefaultAsync();
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

        private void AddRoleInterviewerToUser(long userId)
        {
            var userHasRoleInterviewer = UserHasRole(userId, StaticRoleNames.Tenants.Interviewer);
            if (!userHasRoleInterviewer)
            {
                var roleId = WorkScope.GetAll<Role, int>()
                    .Where(s => s.NormalizedName == StaticRoleNames.Tenants.Interviewer.ToUpper())
                    .Select(s => s.Id)
                    .FirstOrDefault();

                WorkScope.InsertAsync<UserRole>(new UserRole
                {
                    RoleId = roleId,
                    UserId = userId
                });
            }
        }

        public async Task<CapabilityCandidateDto> UpdateCapabilityCV(UpdateCapabilityCandidateDto input)
        {
            var capabilityResult = await WorkScope.GetAsync<RequestCVCapabilityResult>(input.Id);

            capabilityResult.Score = input.Score;
            capabilityResult.Note = input.Note;
            await WorkScope.UpdateAsync(capabilityResult);

            CurrentUnitOfWork.SaveChanges();

            return await IQGetCapability()
                .Where(q => q.Id == capabilityResult.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CapabilityCandidateDto>> UpdateCapabilityCV(List<UpdateCapabilityCandidateDto> input)
        {
            foreach (var item in input)
            {
                var capabilityResult = await WorkScope.GetAsync<RequestCVCapabilityResult>(item.Id);
                capabilityResult.Score = item.Score;
                capabilityResult.Note = item.Note;
                await WorkScope.UpdateAsync(capabilityResult);
            }

            CurrentUnitOfWork.SaveChanges();

            var capabilityResultIds = input.Select(i => i.Id).ToList();

            return await IQGetCapability()
                .Where(q => capabilityResultIds.Contains(q.Id))
                .ToListAsync();
        }

        public async Task<List<CapabilityCandidateDto>> UpdateFactorsCapabilityCV(List<UpdateCapabilityCandidateDto> input)
        {
            foreach (var item in input)
            {
                var capabilityResult = await WorkScope.GetAsync<RequestCVCapabilityResult>(item.Id);
                capabilityResult.Factor = item.Factor;
                await WorkScope.UpdateAsync(capabilityResult);
            }

            CurrentUnitOfWork.SaveChanges();

            var capabilityResultIds = input.Select(i => i.Id).ToList();

            return await IQGetCapability()
                .Where(q => capabilityResultIds.Contains(q.Id))
                .ToListAsync();
        }

        public async Task<ApplicationResultCandidateDto> UpdateApplicationResult(UpdateApplicationResultDto input)
        {
            if (input.Status == RequestCVStatus.Onboarded && !input.OnboardDate.HasValue)
                throw new UserFriendlyException("Onboard Date is required.");

            var entity = WorkScope.GetAll<RequestCV>().Where(s => s.Id == input.RequestCvId)
                .Select(s => new
                {
                    RequestCV = s,
                    CV = s.CV,
                    Status = s.Status,
                    EmailSent = s.EmailSent,
                    HRNote = s.HRNote
                }).FirstOrDefault();
            input.EmailSent = entity.EmailSent;

            if (entity == default)
            {
                throw new UserFriendlyException("Not found RequestCV with Id " + input.RequestCvId);
            }
            var getCv = entity.CV;
            var applicationResult = entity.RequestCV;
            if (getCv?.Note != input?.HRNote && input?.HRNote != null)
            {
                getCv.Note = input.HRNote;
            }
            var oldStatus = applicationResult.Status;
            var oldOnboardDate = applicationResult.OnboardDate;
            if (input.Status == RequestCVStatus.Onboarded || applicationResult.Status != input.Status)
            {
                input.EmailSent = false;
                applicationResult.Status = input.Status;
                await CreateRequestCVHistory(new HistoryRequestCVDto
                {
                    Id = applicationResult.Id,
                    Status = input.Status,
                    OnboardDate = input.OnboardDate,
                });
                await CreateRequestCVStatusChangeHistory(new StatusChangeRequestCVDto
                {
                    Id = applicationResult.Id,
                    FromStatus = entity.Status,
                    ToStatus = input.Status,
                });

                var cv = entity.CV;
                if (applicationResult.FinalLevel.HasValue)
                {
                    var newUserType = applicationResult.FinalLevel < Level.FresherMinus ? UserType.Intern : UserType.Staff;
                    if (cv.UserType != newUserType)
                    {
                        cv.UserType = newUserType;
                    }
                }
            }

            ObjectMapper.Map(input, applicationResult);

            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.Status == RequestCVStatus.AcceptedOffer || input.Status == RequestCVStatus.RejectedOffer)
            {
                UpdateHrmTempEmployee(applicationResult.Id);
            }

            if (IsSendNotifyAcceptOrRejectOffer(oldStatus, input.Status)
                || (input.Status == RequestCVStatus.AcceptedOffer && Nullable.Compare<DateTime>(oldOnboardDate, input.OnboardDate) != 0))
            {
                var isFirstAcceptedOffer = (oldStatus != RequestCVStatus.AcceptedOffer && input.Status == RequestCVStatus.AcceptedOffer)
                    && (!oldOnboardDate.HasValue && input.OnboardDate.HasValue);

                _komuNotification.NotifyAcceptedOrRejectedOffer(applicationResult.Status, applicationResult.Id, isFirstAcceptedOffer);
            }

            return await GetApplicationResultByRequestCVId(applicationResult.Id);
        }

        public async Task<InterviewLevelCandidateDto> UpdateInterviewLevel(UpdateInterviewLevelDto input)
        {
            var entity = WorkScope.GetAll<RequestCV>().Where(s => s.Id == input.RequestCvId)
                .Select(s => new
                {
                    RequestCV = s,
                    CV = s.CV,
                }).FirstOrDefault();

            if (entity == default)
            {
                throw new UserFriendlyException("Not found RequestCV with Id " + input.RequestCvId);
            }
            var applicationResult = entity.RequestCV;
            ObjectMapper.Map(input, applicationResult);
            return await GetInterviewLevelByRequestCVId(applicationResult.Id);
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

        private bool IsSendNotifyAcceptOrRejectOffer(RequestCVStatus oldStatus, RequestCVStatus newStatus)
        {
            if (oldStatus == newStatus)
                return false;
            if ((oldStatus == RequestCVStatus.AcceptedOffer || oldStatus == RequestCVStatus.Onboarded) && newStatus == RequestCVStatus.RejectedOffer)
                return true;
            if (newStatus == RequestCVStatus.AcceptedOffer && oldStatus != RequestCVStatus.Onboarded)
                return true;
            if (newStatus == RequestCVStatus.Onboarded && oldStatus == RequestCVStatus.RejectedOffer)
                return true;
            return false;
        }

        public async Task<ApplicationResultCandidateDto> GetApplicationResultByRequestCVId(long requestCvId)
        {
            var applicationResult = await WorkScope.GetAll<RequestCV>()
                .Where(q => q.Id == requestCvId)
                .Select(s => new ApplicationResultCandidateDto
                {
                    CVId = s.CVId,
                    ApplyLevel = s.ApplyLevel,
                    FinalLevel = s.FinalLevel,
                    InterviewLevel = s.InterviewLevel,
                    HRNote = s.HRNote,
                    OnboardDate = s.OnboardDate,
                    Salary = s.Salary,
                    Status = s.Status,
                    Percentage = s.Percentage,
                    HistoryStatuses = s.RequestCVStatusHistories
                    .OrderByDescending(s => s.TimeAt)
                    .Select(x => new HistoryStatusesDto
                    {
                        Status = x.Status,
                        TimeAt = x.TimeAt
                    }).ToList(),
                    LMSInfo = s.LMSInfo,
                    HistoryChangeStatuses = s.RequestCVStatusChangeHistoies
                    .OrderByDescending(s => s.TimeAt)
                    .Select(x => new HistoryChangeStatusesDto
                    {
                        FromStatus = x.FromStatus,
                        ToStatus = x.ToStatus,
                        TimeAt = x.TimeAt
                    }).ToList(),
                })
                .FirstOrDefaultAsync();

            var controlSendMail = await CheckAllowSendMail(requestCVId: requestCvId);
            applicationResult.MailDetail = new MailDetailDto
            {
                IsAllowSendMail = controlSendMail == ControlSendMail.DENY ? false : true,
                IsSentMailStatus = controlSendMail == ControlSendMail.WARN ? true : false,
                MailStatusHistories = await _mailService.GetMailStatusHistoryByCVId(applicationResult.CVId)
            };

            return applicationResult;
        }

        public async Task<InterviewLevelCandidateDto> GetInterviewLevelByRequestCVId(long requestCvId)
        {
            var applicationResult = await WorkScope.GetAll<RequestCV>()
                .Where(q => q.Id == requestCvId)
                .Select(s => new InterviewLevelCandidateDto
                {
                    CVId = s.CVId,
                    InterviewLevel = s.InterviewLevel,
                })
                .FirstOrDefaultAsync();
            return applicationResult;
        }
        public async Task<InterviewedDto> GetInterviewedByRequestCVId(long requestCvId)
        {
            var applicationResult = await WorkScope.GetAll<RequestCV>()
                .Where(q => q.Id == requestCvId)
                .Select(s => new InterviewedDto
                {
                    CVId = s.CVId,
                    InterviewLevel = s.InterviewLevel,
                    Score = s.RequestCVCapabilityResults
                        .Where(r => r.Capability.FromType == false)
                        .Where(r => r.Factor > 0)
                        .Select(r => r.Score)
                        .ToList(),
                })
                .FirstOrDefaultAsync();
            return applicationResult;
        }
        public async Task CreateRequestCVHistory(HistoryRequestCVDto input)
        {
            var oppositionStatuses = CommonUtils.GetOppositionStatus(input.Status);
            var histories = await WorkScope.GetAll<RequestCVStatusHistory>()
                .Where(q => q.RequestCVId == input.Id)
                .Where(q => q.Status == input.Status || (oppositionStatuses != default ? oppositionStatuses.Contains(q.Status) : false))
                .ToListAsync();

            DateTime timeAt = DateTimeUtils.GetNow();
            if (input.Status == RequestCVStatus.Onboarded)
            {
                timeAt = input.OnboardDate.Value.AddTicks(DateTimeUtils.GetTickTime(DateTimeUtils.GetNow()));
            }

            if (histories == null || histories.Count == 0)
            {
                var historyRequestCv = new RequestCVStatusHistory
                {
                    RequestCVId = input.Id,
                    Status = input.Status,
                    TimeAt = timeAt
                };

                await WorkScope.InsertAsync(historyRequestCv);
            }
            else
            {
                int countHistory = histories.Count;
                if (countHistory > 1)
                {
                    histories[0].Status = input.Status;
                    histories[0].TimeAt = timeAt;
                    await WorkScope.UpdateAsync(histories[0]);
                    for (int i = 1; i < countHistory; i++)
                    {
                        await WorkScope.DeleteAsync(histories[i]);
                    }
                }
                else
                {
                    if (oppositionStatuses != default && histories[0].Status != input.Status)
                        histories[0].Status = input.Status;

                    histories[0].TimeAt = timeAt;
                    await WorkScope.UpdateAsync(histories[0]);
                }
            }
            CurrentUnitOfWork.SaveChanges();
        }

        public async Task CreateRequestCVStatusChangeHistory(StatusChangeRequestCVDto input)
        {
            DateTime timeAt = DateTimeUtils.GetNow();
            var requestCVStatusChangeHistory = new RequestCVStatusChangeHistory
            {
                RequestCVId = input.Id,
                FromStatus = input.FromStatus,
                ToStatus = input.ToStatus,
                TimeAt = timeAt
            };
            await WorkScope.InsertAsync(requestCVStatusChangeHistory);
            CurrentUnitOfWork.SaveChanges();
        }

        public async Task<List<InterviewCandidateDto>> GetInterviewerCVsByRequestCVId(long requestCvId)
        {
            var interviewers = await IQGetInterviewCVs()
                .Where(q => q.RequestCvId == requestCvId)
                .ToListAsync();
            return interviewers;
        }

        public IQueryable<InterviewCandidateDto> IQGetInterviewCVs()
        {
            var query = WorkScope.GetAll<RequestCVInterview>()
               .Select(s => new InterviewCandidateDto
               {
                   Id = s.Id,
                   RequestCvId = s.RequestCVId,
                   InterviewId = s.Interview.Id,
                   InterviewName = s.Interview.FullName,
                   EmailAddress = s.Interview.EmailAddress
               });
            return query;
        }

        public IQueryable<CandidateDto> IQGetAllCVs()
        {
            var now = DateTimeUtils.GetNow();
            var query = from candidate in WorkScope.GetAll<CV>()
                        select new CandidateDto
                        {
                            Id = candidate.Id,
                            UserType = candidate.UserType,
                            PathAvatar = candidate.Avatar,
                            PathLinkCV = candidate.LinkCV,
                            Email = candidate.Email,
                            FullName = candidate.Name,
                            Phone = candidate.Phone,
                            Note = candidate.Note,
                            IsFemale = candidate.IsFemale,
                            CvStatus = candidate.CVStatus,
                            SubPositionId = candidate.SubPositionId,
                            SubPositionName = candidate.SubPosition.Name,
                            BranchId = candidate.BranchId,
                            BranchName = candidate.Branch.Name,
                            CreatorUserId = candidate.CreatorUserId,
                            CreationTime = candidate.CreationTime,
                            CreatorName = candidate.CreatorUserId.HasValue ?
                                    candidate.CreatorUser.FullName :
                                    "",
                            LastModifiedTime = candidate.LastModificationTime,
                            LastModifiedName = candidate.LastModifierUserId.HasValue ?
                                        candidate.LastModifierUser.FullName :
                                        "",
                            ProcessCVStatus = (candidate.CVStatus == CVStatus.New || candidate.CVStatus == CVStatus.Contacting) ?
                            (
                            (candidate.LastModificationTime.HasValue ? (now - candidate.LastModificationTime.Value).TotalDays : (now - candidate.CreationTime).TotalDays) <= 1 ? ProcessCVStatus.UnprocessedCV : ProcessCVStatus.OverdueCV
                            )
                            : ProcessCVStatus.None,
                            CVSkills = candidate.CVSkills.Select(x => new SkillCandidatePagingDto
                            {
                                Id = x.SkillId,
                                Level = x.Level,
                                Name = x.Skill.Name
                            }).ToList(),
                            RequisitionInfos = candidate.RequestCVs
                            .Where(s => !s.Request.IsDeleted)
                            .OrderByDescending(s => s.LastModificationTime)
                            .Select(x => new RequisitionInfoDto
                            {
                                BranchId = x.Request.BranchId,
                                BranchName = x.Request.Branch.Name,
                                DisplayBranchName = x.Request.Branch.DisplayName,
                                Id = x.RequestId,
                                SubPositionId = x.Request.SubPositionId,
                                Note = x.HRNote,
                                Level = x.Request.Level,
                                SubPositionName = x.Request.SubPosition.Name,
                                SkillRequests = x.Request.RequestSkills.Select(x => new CategoryDto
                                {
                                    Id = x.SkillId,
                                    Name = x.Skill.Name
                                }).ToList(),
                                RequestStatus = x.Request.Status,
                                RequestCVStatus = x.Status,
                                LastModifiedTime = x.LastModificationTime,
                                ProjectToolRequestId = x.Request.ProjectToolRequestId,
                                CapabilityResults = x.RequestCVCapabilityResults.Select(c => new RequestCVCapabilityResultDto
                                {
                                    Score = c.Score,
                                    Factor = c.Factor,
                                }).ToList(),
                                InterviewLevel = x.InterviewLevel,
                                ApplyLevel = x.ApplyLevel,
                                FinalLevel = x.FinalLevel,
                                InterviewTime = x.InterviewTime,
                            })
                            .ToList(),
                            MailStatusHistories = candidate.EmailStatusHistories.Select(s => new MailStatusHistoryDto
                            {
                                CVId = s.CVId,
                                CreationTime = s.CreationTime,
                                Description = s.Description,
                                Id = s.Id,
                                MailFuncType = s.EmailTemplate.Type,
                                Subject = s.EmailTemplate.Subject
                            })
                            .OrderByDescending(q => q.CreationTime)
                            .ToList(),
                            HistoryChangeStatuses = candidate.RequestCVs.OrderByDescending(x => x.CreationTime).FirstOrDefault().RequestCVStatusChangeHistoies.Select(x => new HistoryChangeStatusesDto
                            {
                                FromStatus = x.FromStatus,
                                ToStatus = x.ToStatus
                            }).ToList(),
                            CVEducations = candidate.CVEducations
                            .Where(CVEducation => !CVEducation.IsDeleted)
                            .Select(CVEducation => new CVEducationDto
                            {
                                Id = CVEducation.Id,
                                EducationName = CVEducation.Education.Name
                            })
                            .ToList(),
                            LatestModifiedTime = candidate.RequestCVs
                            .Where(s => !s.Request.IsDeleted)
                            .Select(s => s.LastModificationTime)
                            .FirstOrDefault() ?? candidate.LastModificationTime,
                            IsDeleted = candidate.IsDeleted,
                        };
            return query;
        }

        public IQueryable<CapabilityCandidateDto> IQGetCapability()
        {
            var query = from x in WorkScope.GetAll<RequestCVCapabilityResult>()
                        select new CapabilityCandidateDto
                        {
                            Id = x.Id,
                            RequestCvId = x.RequestCV.Id,
                            CapabilityId = x.Capability.Id,
                            CapabilityName = x.Capability.Name,
                            Note = x.Note,
                            Score = x.Score,
                            Factor = x.Factor,
                            FromType = x.Capability.FromType
                        };
            return query;
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
                        where cv.CreatorUserId.HasValue && cv.UserType == userType && !cv.CreatorUser.IsDeleted && cv.CreatorUser.IsActive
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

        public async Task UpdateInterviewTime(UpdateInterviewTimeDto input)
        {
            var requestCv = await WorkScope.GetAsync<RequestCV>(input.RequestCVId);
            requestCv.InterviewTime = input.InterviewTime;
            await WorkScope.UpdateAsync(requestCv);
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

        public async Task<List<HistoryCandidateDto>> GetHistoryCV(long cvId)
        {
            var histories = await WorkScope.GetAll<RequestCV>()
                .Where(q => q.CVId == cvId)
                .Select(s => new HistoryCandidateDto
                {
                    BranchId = s.Request.BranchId,
                    BranchName = s.Request.Branch.Name,
                    RequestId = s.Request.Id,
                    Level = s.Request.Level,
                    SubPositionId = s.Request.SubPositionId,
                    SubPositionName = s.Request.SubPosition.Name,
                    Priority = s.Request.Priority,
                    Skills = s.Request.RequestSkills.Select(s => new SkillDto
                    {
                        Id = s.SkillId,
                        Name = s.Skill.Name
                    }).ToList(),
                    Quantity = s.Request.Quantity,
                    RequestStatus = s.Request.Status,
                    UserType = s.Request.UserType,
                    Note = s.Request.Note,
                    TimeNeed = s.Request.TimeNeed,
                    StatusHistories = s.RequestCVStatusHistories.Select(x => new StatusHistoryDto
                    {
                        TimeAt = x.TimeAt,
                        CreatorName = x.CreatorUser.Name,
                        LastModifiedName = x.LastModifierUser.Name,
                        RequestCVStatus = x.Status,
                        LastModifiedTime = x.LastModificationTime
                    }).OrderByDescending(q => q.TimeAt).ToList(),
                    CreatorUserId = s.Request.CreatorUserId,
                    CreationTime = s.Request.CreationTime,
                    CreatorName = s.Request.CreatorUserId.HasValue ?
                                    s.Request.CreatorUser.FullName :
                                    "",
                    LastModifiedTime = s.Request.LastModificationTime,
                    LastModifiedName = s.Request.LastModifierUserId.HasValue ?
                                        s.Request.LastModifierUser.FullName :
                                        "",
                    TimeClose = s.Request.Status == StatusRequest.Closed ? s.Request.LastModificationTime : null,
                    CreationTimeRequestCV = s.CreationTime,
                    ProjectToolRequestId = s.Request.ProjectToolRequestId
                })
                .OrderByDescending(x => x.CreationTimeRequestCV)
                .ToListAsync();
            return histories;
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

        public async Task<UpdateCandidateNoteDto> UpdateNote(UpdateCandidateNoteDto input)
        {
            var cv = await WorkScope.GetAsync<CV>(input.CVId);
            cv.Note = input.Note;
            await CurrentUnitOfWork.SaveChangesAsync();
            return input;
        }

        #region export infomation

        public async Task<FileContentResult> ExportReport(ExportReport input)
        {
            var clientUrl = _configuration.GetValue<string>($"App:ClientRootAddress");
            var bulletPoint = "\u002B" + "\x20";

            var exportCandidates = await IQGetAllCVs()
               .Where(q => q.UserType.Equals(input.userType) && !q.IsDeleted)
               .WhereIf(input.reqCvStatus.HasValue, q => q.RequisitionInfos.Any(s => s.RequestCVStatus == input.reqCvStatus))
               .WhereIf(input.FromStatus.HasValue, q => q.HistoryChangeStatuses.Any(s => s.FromStatus == input.FromStatus))
               .WhereIf(input.ToStatus.HasValue, q => q.HistoryChangeStatuses.Any(s => s.ToStatus == input.ToStatus))
               .WhereIf(input.FromDate.HasValue, q => q.LastModifiedTime.Value.Date >= input.FromDate.Value.Date)
               .WhereIf(input.ToDate.HasValue, q => q.LastModifiedTime.Value.Date < input.ToDate.Value.Date.AddDays(1))
               .ToListAsync();

            var candidatesResult = exportCandidates
                .Select((u, index) => new CandidateReport
                {
                    No = 1 + index++,
                    Name = u.FullName,
                    Phone = u.Phone,
                    Email = u.Email,
                    Sex = u.IsFemale ? "Male" : "Female",
                    CvStatus = u.CvStatus,
                    Education = string.Join(Environment.NewLine, u.CVEducations.Select(e => bulletPoint + e.EducationName)),
                    Branch = u.BranchName,
                    Positon = u.SubPositionName,
                    Status = u.RequisitionInfos.FirstOrDefault()?.RequestCVStatus,
                    Time = u.LastModifiedTime,
                    ApplyLevel = GetLevelStandardName(u.RequisitionInfos.FirstOrDefault()?.ApplyLevel),
                    FinalLevel = GetLevelStandardName(u.RequisitionInfos.FirstOrDefault()?.FinalLevel),
                    InterviewLevel = GetLevelStandardName(u.RequisitionInfos.FirstOrDefault()?.InterviewLevel),
                    Score = CalculateScore(u.RequisitionInfos.FirstOrDefault()?.CapabilityResults),
                    Note = u.Note,
                    TalentLink = clientUrl + "app/candidate/" + (u.UserType == UserType.Staff ? "staff-list" : "intern-list") + $"/{u.Id}?userType={(int)u.UserType}&tab=3"
                })
                .ToList();

            List<RequestCVStatus> interviewedRequestCVStatus = new List<RequestCVStatus>
            {
                RequestCVStatus.ScheduledInterview,
                RequestCVStatus.RejectedInterview,
                RequestCVStatus.PassedInterview,
                RequestCVStatus.FailedInterview,
                RequestCVStatus.AcceptedOffer,
                RequestCVStatus.RejectedOffer,
                RequestCVStatus.Onboarded
            };
            var interviewedResult = exportCandidates
                .Where(candidate =>
                {
                    var requestCVStatus = candidate.RequisitionInfos.FirstOrDefault()?.RequestCVStatus;
                    if (requestCVStatus.HasValue) return interviewedRequestCVStatus.Contains(requestCVStatus.Value);
                    return false;
                })
                .Select((u, index) => new InterviewReport
                {
                    No = 1 + index++,
                    Name = u.FullName,
                    Email = u.Email,
                    Branch = u.BranchName,
                    Positon = u.SubPositionName,
                    Status = u.RequisitionInfos.FirstOrDefault()?.RequestCVStatus,
                    Time = u.RequisitionInfos.FirstOrDefault()?.InterviewTime,
                    ApplyLevel = GetLevelStandardName(u.RequisitionInfos.FirstOrDefault()?.ApplyLevel),
                    FinalLevel = GetLevelStandardName(u.RequisitionInfos.FirstOrDefault()?.FinalLevel),
                    InterviewLevel = GetLevelStandardName(u.RequisitionInfos.FirstOrDefault()?.InterviewLevel),
                    Score = CalculateScore(u.RequisitionInfos.FirstOrDefault()?.CapabilityResults),
                    TalentLink = clientUrl + "app/candidate/" + (u.UserType == UserType.Staff ? "staff-list" : "intern-list") + $"/{u.Id}?userType={(int)u.UserType}&tab=3"
                }
                )
                .ToList();

            using (var package = new ExcelPackage())
            {
                var startRow = 1;
                var startColumn = 1;
                var columnKey = GetColumnNameFromNumber(startColumn);

                var candidatesReportWorksheet = package.Workbook.Worksheets.Add("CandidatesReport");
                candidatesReportWorksheet.Cells[$"{columnKey}{startRow}"].LoadFromCollection(candidatesResult, true, TableStyles.Light9);
                var reportRow = candidatesReportWorksheet.Dimension.Start.Row;
                candidatesReportWorksheet.Column(8).Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                candidatesResult.ForEach(candidate =>
                {
                    var talentLinkCell = candidatesReportWorksheet.Cells[++reportRow, candidatesReportWorksheet.Dimension.End.Column];
                    if (!string.IsNullOrWhiteSpace(candidate.TalentLink))
                    {
                        talentLinkCell.Hyperlink = new ExcelHyperLink(candidate.TalentLink) { Display = "Talent link" };
                        talentLinkCell.Style.Font.UnderLine = true;
                        talentLinkCell.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    }
                });
                candidatesReportWorksheet.Cells.AutoFitColumns();
                candidatesReportWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var interviewedReportWorksheet = package.Workbook.Worksheets.Add("InterviewedReport");
                interviewedReportWorksheet.Cells[$"{columnKey}{startRow}"].LoadFromCollection(interviewedResult, true, TableStyles.Light9);
                var interviewedRow = interviewedReportWorksheet.Dimension.Start.Row;
                interviewedReportWorksheet.Column(4).Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                interviewedResult.ForEach(interview =>
                {
                    var talentLinkCell = interviewedReportWorksheet.Cells[++interviewedRow, interviewedReportWorksheet.Dimension.End.Column];
                    if (!string.IsNullOrWhiteSpace(interview.TalentLink))
                    {
                        talentLinkCell.Hyperlink = new ExcelHyperLink(interview.TalentLink) { Display = "Talent link" };
                        talentLinkCell.Style.Font.UnderLine = true;
                        talentLinkCell.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    }
                });
                interviewedReportWorksheet.Cells.AutoFitColumns();
                interviewedReportWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var stream = new MemoryStream();
                package.SaveAs(stream);
                return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = input.userType + "Report.xlsx"
                };
            }
        }

        private double? CalculateScore(List<RequestCVCapabilityResultDto> capabilityResults)
        {
            if (capabilityResults != null && capabilityResults.Count > 0)
            {
                double evaluationScore = 0;
                double totalScore = 0;
                capabilityResults.ForEach(result =>
                {
                    evaluationScore += (result.Score * result.Factor);
                    totalScore += (5 * result.Factor);
                });
                return Math.Round((evaluationScore / totalScore) * 5, 2);
            }
            return null;
        }

        private string GetLevelStandardName(Level? level)
        {
            if (level.HasValue) return DictionaryHelper.LevelDict[level.Value]?.StandardName;
            return null;
        }

        private string GetColumnNameFromNumber(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
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

        public async Task<MailPreviewInfoDto> PreviewBeforeSendMailRequestCV(long requestCVId, string? mailVersion = null)
        {
            if (string.IsNullOrEmpty(mailVersion))
            {
                return await _mailService.GetContentMailRequestCV(requestCVId);
            }
            else
            {
                return await _mailService.GetContentMailRequestCV(requestCVId, mailVersion);
            }
        }

        public async Task<MailDetailDto> SendMailCV(long cvId, MailPreviewInfoDto message)
        {
            if (await CheckAllowSendMail(cvId: cvId) == ControlSendMail.DENY)
                throw new UserFriendlyException("Can't send email. Please, try again!");

            _mailService.Send(message);
            var description = $"Sent {message.Subject}";
            await _mailService.CreateEmailHistory(cvId, message.TemplateId, description);
            CurrentUnitOfWork.SaveChanges();

            var controlSendMail = await CheckAllowSendMail(cvId: cvId);
            return new MailDetailDto
            {
                MailStatusHistories = await _mailService.GetMailStatusHistoryByCVId(cvId),
                IsAllowSendMail = controlSendMail == ControlSendMail.DENY ? false : true,
                IsSentMailStatus = controlSendMail == ControlSendMail.WARN ? true : false
            };
        }

        public async Task<MailDetailDto> SendMailRequestCV(long requestCVId, MailPreviewInfoDto message)
        {
            if (await CheckAllowSendMail(requestCVId: requestCVId) == ControlSendMail.DENY)
                throw new UserFriendlyException("Can't send email. Please, try again!");

            _mailService.Send(message);

            var requestCVs = WorkScope.GetAll<RequestCV>()
              .Where(q => q.Id == requestCVId);
            var requestCVDto = requestCVs
               .Select(s => new
               {
                   s.CVId,
                   s.RequestId,
                   BranchName = s.Request.Branch.Name,
                   SubPositionName = s.Request.SubPosition.Name,
                   s.Request.UserType,
                   s.EmailSent
               })
               .FirstOrDefault();
            var requestCv = requestCVs.FirstOrDefault();
            requestCv.EmailSent = true;
            var description = $"Request #{requestCVDto.RequestId} {requestCVDto.BranchName} {CommonUtils.GetEnumName(requestCVDto.UserType)} {requestCVDto.SubPositionName}";
            await _mailService.CreateEmailHistory(requestCVDto.CVId, message.TemplateId, description);
            CurrentUnitOfWork.SaveChanges();

            var controlSendMail = await CheckAllowSendMail(requestCVId: requestCVId);
            return new MailDetailDto
            {
                IsAllowSendMail = controlSendMail == ControlSendMail.DENY ? false : true,
                IsSentMailStatus = controlSendMail == ControlSendMail.WARN ? true : false,
                MailStatusHistories = await _mailService.GetMailStatusHistoryByCVId(requestCVDto.CVId)
            };
        }

        public async Task<string> CreateAccountStudent(long cvId, long requestCVId, StatusCreateAccount statusCreateAccount)
        {
            var cv = WorkScope.GetAll<CV>()
                .Where(q => q.Id == cvId)
                .Select(s => new
                {
                    Name = s.Name,
                    SubPositionName = s.SubPosition.Name,
                    UserType = s.UserType,
                    SubPositionId = s.SubPositionId,
                    EmailAddress = s.Email,
                    BranchDisplayName = s.Branch.DisplayName ?? s.Branch.Name,
                }).FirstOrDefault();

            var urlContest = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.TalentContestUrl);
            var accountStudent = new StudentDto
            {
                EmailAddress = cv.EmailAddress,
                FullName = cv.Name,
                Name = Utils.StringExtensions.GetNamePerson(cv.Name),
                Surname = Utils.StringExtensions.GetSurnamePerson(cv.Name),
                Password = PasswordUtils.GeneratePassword(6, true),
                UserName = Utils.StringExtensions.GetAccountUserLMS(cv.Name, cv.UserType.ToString(), cv.SubPositionName, cv.BranchDisplayName)
            };
            var requestCV = await WorkScope.GetAsync<RequestCV>(requestCVId);
            if (statusCreateAccount == StatusCreateAccount.CREATE_LMS_ACCOUT)
            {
                var course = WorkScope.GetAll<PositionSetting>()
                .Where(q => q.UserType == cv.UserType && q.SubPositionId == cv.SubPositionId)
                .Select(s => new { s.LMSCourseId, s.LMSCourseName })
                .FirstOrDefault();

              if (course == null || !course.LMSCourseId.HasValue)
                throw new UserFriendlyException($"Not Found Course With UserType: {CommonUtils.GetEnumName(cv.UserType)} and SubPosition {cv.SubPositionName}");
                accountStudent.CourseInstanceId = course.LMSCourseId.Value;

              var newStudent = await _lmsService.CreateAccountStudent(accountStudent);
                if (newStudent == null)
                    throw new UserFriendlyException("Create Account From LMS Failed! Please again.");
                requestCV.LMSInfo = TemplateHelper.ContentLMSInfo(newStudent.UserName, newStudent.Password, course.LMSCourseName, newStudent.CourseInstanceId);
                CurrentUnitOfWork.SaveChanges();
                return requestCV.LMSInfo;
            }
            else
            {
                requestCV.LMSInfo = TemplateHelper.ContestUrl(urlContest);
                return requestCV.LMSInfo;
            }
        }

        public async Task<ControlSendMail> CheckAllowSendMail(
            long cvId = default,
            long requestCVId = default
        )
        {
            if (cvId != default)
            {
                return await CheckAllowSendMailFailedCV(cvId);
            }
            else if (requestCVId != default)
            {
                return await CheckAllowSendMailRequestCV(requestCVId);
            }
            throw new NotImplementedException("Logic isn't implemented.");
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

        private async Task<ControlSendMail> CheckAllowSendMailRequestCV(long requestCVId)
        {
            var requestCV = await WorkScope.GetAll<RequestCV>()
                .Where(q => q.Id == requestCVId)
                .Select(s => new { s.CVId, s.Status, s.CV.UserType })
                .FirstOrDefaultAsync();

            var mailFuncType = CommonUtils.MapRequestCVStatusToEmailType(requestCV.Status, requestCV.UserType);
            if (mailFuncType == default)
                return ControlSendMail.DENY;

            if (await _mailService.IsSentMailCV(requestCV.CVId, mailFuncType))
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
        public async Task<InterviewedDto> UpdateInterviewed(UpdateInterviewedDto input)
        {
            var entity = WorkScope.GetAll<RequestCV>().Where(s => s.Id == input.RequestCvId)
                .Select(s => new
                {
                    RequestCV = s,
                    CV = s.CV,
                }).FirstOrDefault();

            if (entity == default)
            {
                throw new UserFriendlyException("Not found RequestCV with Id " + input.RequestCvId);
            }
            var applicationResult = entity.RequestCV;
            ObjectMapper.Map(input, applicationResult);
            return await GetInterviewedByRequestCVId(applicationResult.Id);
        }
        #endregion send mail CV
    }
}