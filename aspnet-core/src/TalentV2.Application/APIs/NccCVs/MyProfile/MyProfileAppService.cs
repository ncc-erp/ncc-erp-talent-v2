using Abp.Authorization;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.APIs.NccCVs.MyProfile.Dto;
using TalentV2.Authorization;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;
using TalentV2.Constants.Const;
using TalentV2.Constants.Enum.NccCVs;
using TalentV2.Entities.NccCVs;
using TalentV2.FileServices.Services.Employees;
using TalentV2.Utils;

namespace TalentV2.APIs.NccCVs.MyProfile
{
    [AbpAuthorize]
    public class MyProfileAppService : TalentV2AppServiceBase
    {
        private readonly IFileEmployeeService _fileEmployeeService;
        public MyProfileAppService(IFileEmployeeService fileEmployeeService)
        {
            _fileEmployeeService = fileEmployeeService;
        }
        [AbpAuthorize(PermissionNames.MyProfile_Certification_Create, PermissionNames.MyProfile_Certification_Edit)]
        public async Task<List<string>> UpdateCertificate(List<string> input)
        {
            var user = await GetCurrentUserAsync();
            user.Certificates = JsonConvert.SerializeObject(input);
            await WorkScope.UpdateAsync(user);
            return input;
        }
        [AbpAuthorize(PermissionNames.MyProfile_Certification_View)]
        public async Task<List<string>> GetCertificateAsync(long userId)
        {
            if (userId != AbpSession.UserId && !(await UserManager.IsGrantedAsync(AbpSession.UserId.Value, PermissionNames.Employee_ViewDetail)))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.ViewOtherProfile);
            }
            var user = await GetCurrentUserAsync();
            return user.Certificates.HasValue() ? JsonConvert.DeserializeObject<List<string>>(user.Certificates) : null;
        }

        public async Task<TechnicalExpertiseDto> GetTechnicalExpertise(long userId)
        {
            if ((AbpSession.UserId.Value != userId) && !await UserManager.IsGrantedAsync(AbpSession.UserId.Value, PermissionNames.Employee_ViewDetail))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.AccessOtherProfile);
            }

            var query = await WorkScope.GetAll<User>()
                                       .Include(u => u.EmployeeSkills)
                                       .ThenInclude(u => u.GroupSkill)
                                       .ThenInclude(cvs => cvs.Skills)
                                       .FirstOrDefaultAsync(u => u.Id == userId);

            if (query == default)
            {
                throw new UserFriendlyException(ErrorCodes.NotFound.UserNotExist);
            }

            var userGroupSkills = query.EmployeeSkills
                                 .GroupBy(s => s.GroupSkillId)
                                 .Select(g => new GroupSkillAndSkillDto
                                 {
                                     GroupSkillId = g.Key.Value,
                                     Name = g.FirstOrDefault().GroupSkill?.Name,
                                     CVSkills = g.Select(s => new CVSkillDto
                                     {
                                         Id = s.Id,
                                         SkillId = s.SkillId,
                                         SkillName = string.IsNullOrWhiteSpace(s.SkillName) ? s.GroupSkill.Skills.FirstOrDefault(sk => sk.Id == s.SkillId)?.Name : s.SkillName,
                                         Level = s.Level,
                                     }).ToList()
                                 })
                                .ToList();

            return new TechnicalExpertiseDto
            {
                UserId = userId,
                GroupSkills = userGroupSkills,
            };
        }
        [AbpAuthorize(PermissionNames.MyProfile_TechnicalExpertise_Create, PermissionNames.MyProfile_TechnicalExpertise_Edit)]
        public async Task UpdateTechnicalExpertise(TechnicalExpertiseDto input)
        {
            if (AbpSession.UserId != input.UserId && !(await UserManager.IsGrantedAsync(AbpSession.UserId.Value, PermissionNames.Employee_ViewDetail)))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.EditOtherProfile);
            }

            var dbCVSkills = WorkScope.GetAll<EmployeeSkill>();
            var groupSkills = WorkScope.GetAll<GroupSkill>();
            List<long> listCurrentId = new List<long>();
            foreach (var grs in input.GroupSkills)
            {
                long? groupSkillId = groupSkills.FirstOrDefault(s => !string.IsNullOrEmpty(s.Name) && s.Name.Trim().ToLower().Contains((grs.Name.HasValue() ? grs.Name : "").Trim().ToLower()))?.Id;

                if (groupSkillId == default)
                {
                    groupSkillId = await WorkScope.InsertOrUpdateAndGetIdAsync(new GroupSkill
                    {
                        Name = grs.Name.FirstLetterToUpper(),
                        Default = false
                    });
                }
                else
                {
                    groupSkillId = grs.GroupSkillId.HasValue ? grs.GroupSkillId : groupSkillId;
                }
                foreach (var s in grs.CVSkills)
                {
                    listCurrentId.Add(await WorkScope.InsertOrUpdateAndGetIdAsync(new EmployeeSkill
                    {
                        CVEmployeeId = input.UserId,
                        GroupSkillId = groupSkillId,
                        Id = s.Id ?? 0,
                        Level = s.Level,
                        SkillId = s.SkillId,
                        SkillName = s.SkillName,
                    }));
                }
            }

            await WorkScope.DeleteAsync<EmployeeSkill>(c => !listCurrentId.Contains(c.Id) && c.CVEmployeeId == input.UserId);
        }
        [AbpAuthorize(PermissionNames.MyProfile_WorkingExperience_View)]
        public async Task<List<WorkingExperienceDto>> GetUserWorkingExperience([Required] long userId, long? versionId)
        {
            if ((AbpSession.UserId.Value != userId) && !await UserManager.IsGrantedAsync(AbpSession.UserId.Value, PermissionNames.Employee_ViewDetail))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.AccessOtherProfile);
            }
            var now = DateTime.Now;
            var datenow = new DateTime(now.Year, now.Month, now.Day);

            var query = (from w in WorkScope.GetRepo<EmployeeWorkingExperience>().GetAllIncluding(k => k.Project)
                                  .Where(w => w.UserId == userId
                                            && w.VersionId == versionId).AsEnumerable()
                         select new WorkingExperienceDto
                         {
                             Id = w.Id,
                             Position = w.Position,
                             ProjectDescription = w.ProjectDescription,
                             ProjectName = w.Project?.Name,
                             Responsibility = w.Responsibilities,
                             EndTime = w.EndTime == null ? datenow : w.EndTime,
                             StartTime = w.StartTime,
                             UserId = w.UserId,
                             Order = w.Order,
                             Technologies = w.Technologies,
                             VersionId = w.VersionId,
                             ProjectId = w.ProjectId.Value,
                         }).AsEnumerable();
            query = query.OrderByDescending(w => w.EndTime).ThenByDescending(w => w.StartTime).ToList();
           
            return query.ToList();
        }
        [AbpAuthorize(PermissionNames.MyProfile_WorkingExperience_Create, PermissionNames.MyProfile_WorkingExperience_Edit, PermissionNames.Employee_EditAsPM)]
        public async Task UpdateWorkingExperience(WorkingExperienceDto input)
        {
            if (AbpSession.UserId != input.UserId && !(await UserManager.IsGrantedAsync(AbpSession.UserId.Value, PermissionNames.Employee_ViewDetail)))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.EditOtherProfile);
            }

            if (input.VersionId.HasValue && input.VersionId > 0)
            {
                if (!(await WorkScope.GetAll<TalentV2.Entities.NccCVs.Versions>().AnyAsync(s => s.EmployeeId == input.UserId && s.Id == input.VersionId)))
                {
                    throw new UserFriendlyException(ErrorCodes.NotFound.VersionOfCV);
                }
            }

            if (!input.StartTime.HasValue)
            {
                throw new UserFriendlyException(ErrorCodes.BadRequest.LackStartTime);
            }
            if (input.ProjectId <= 0)
            {
                var isExist = await WorkScope.GetAll< TalentV2.Entities.NccCVs.Project >().AnyAsync(s => s.Name == input.ProjectName && s.Type == ProjectType.CreatebyPM);
                if (!isExist)
                {
                    var project = new TalentV2.Entities.NccCVs.Project
                    {
                        Name = input.ProjectName,
                        Description = input.ProjectDescription,
                        Technology = input.Technologies,
                        Type = ProjectType.CreatebyUser
                    };
                    input.ProjectId = await WorkScope.InsertAndGetIdAsync<TalentV2.Entities.NccCVs.Project>(project);
                }
            }
            else
            {
                var project = await WorkScope.GetAsync<TalentV2.Entities.NccCVs.Project>(input.ProjectId);
                if (project.Name != input.ProjectName)
                {
                    input.ProjectId = await WorkScope.InsertAndGetIdAsync<TalentV2.Entities.NccCVs.Project>(new TalentV2.Entities.NccCVs.Project
                    {
                        Name = input.ProjectName,
                        Technology = input.Technologies,
                        Description = input.ProjectDescription,
                        Type = ProjectType.CreatebyUser
                    });
                }
            }


            long Id = await WorkScope.InsertOrUpdateAndGetIdAsync(ObjectMapper.Map<EmployeeWorkingExperience>(input));
            var xx = WorkScope.GetAll<EmployeeWorkingExperience>()
               .Where(x => x.Id == Id)
               .Select(x => new
               {
                   x.Id,
                   x.StartTime,
                   x.EndTime
               });
            if (input.ListOfTechnologies != null)
            {
                var ListTechInput = input.ListOfTechnologies.Select(s => s.TechId);
                var ListOfProjectTechnologies = WorkScope.GetAll<ProjectTechnology>().Where(s => s.ProjectId == input.ProjectId).Select(s => s.TechnologyId);
                var ListOfOldWorkingExpTech = await WorkScope.GetAll<EmployeeWorkingExpAndTechnologies>().Where(s => s.WorkingExpId == Id).Select(s => s.TechnologyId).ToListAsync();
                //add Tech to project
                var NewTechOfProject = ListTechInput.Except(ListOfProjectTechnologies);
                var ListTechAddToProject = new List<ProjectTechnology>();
                foreach (var i in NewTechOfProject)
                {
                    ListTechAddToProject.Add(new ProjectTechnology { ProjectId = input.ProjectId, TechnologyId = i, Status = false });
                }
                await WorkScope.InsertRangeAsync(ListTechAddToProject);
                //AddTechToExp
                var ListTechAddToExp = ListTechInput.Except(ListOfOldWorkingExpTech);
                var ListTechAddToExpDto = new List<EmployeeWorkingExpAndTechnologies>();
                foreach (var i in ListTechAddToExp)
                {
                    ListTechAddToExpDto.Add(new EmployeeWorkingExpAndTechnologies { TechnologyId = i, WorkingExpId = Id });
                }
                await WorkScope.InsertRangeAsync(ListTechAddToExpDto);
                //Remove Tech from exp
                var ListTechRemoveFromExp = ListOfOldWorkingExpTech.Except(ListTechInput);
                var DataToRemove = await WorkScope.GetAll<EmployeeWorkingExpAndTechnologies>().Where(s => ListTechRemoveFromExp.Contains(s.TechnologyId)).ToListAsync();
                foreach (var i in DataToRemove)
                {
                    await WorkScope.DeleteAsync(i);
                }
            }

        }
        [AbpAuthorize(PermissionNames.Employee_EditAsPM, PermissionNames.MyProfile_WorkingExperience_Delete)]
        public async Task DeleteWorkingExperience(long id)
        {
            var exp = await WorkScope.GetAll<EmployeeWorkingExperience>().FirstOrDefaultAsync(e => e.Id == id);
            var currentUser = await GetCurrentUserAsync();
            var checkRoleToDoWork = await UserManager.IsInRoleAsync(currentUser, StaticRoleNames.Tenants.PM) || await UserManager.IsInRoleAsync(currentUser, StaticRoleNames.Tenants.Admin);
            if (AbpSession.UserId != exp.UserId && !checkRoleToDoWork)
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.EditOtherProfile);
            }

            await WorkScope.DeleteAsync<EmployeeWorkingExperience>(id);
            var ListOfTechInWorkingExp = WorkScope.GetAll<EmployeeWorkingExpAndTechnologies>().Where(s => s.WorkingExpId == id);
            foreach (var i in ListOfTechInWorkingExp)
            {
                await WorkScope.DeleteAsync(i);
            }
        }

        public async Task<UserGeneralInfoDto> GetUserGeneralInfo(long userId, long? versionId)
        {
            if ((AbpSession.UserId.Value != userId) && !await UserManager.IsGrantedAsync(AbpSession.UserId.Value, PermissionNames.Employee_ViewDetail))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.AccessOtherProfile);
            }

            var userGeneralInfo = await WorkScope.GetAll<User, long>()
                                                 .Include(u => u.Position)
                                                 .Include(u => u.Branch)
                                                 .Include(u => u.Versions)
                                                 .FirstOrDefaultAsync(u => u.Id == userId);

            if (versionId.HasValue && versionId > 0 && !userGeneralInfo.Versions.Any(s => s.Id == versionId))
            {
                throw new UserFriendlyException(ErrorCodes.NotFound.VersionOfCV);
            }

            return new UserGeneralInfoDto
            {
                UserId = userGeneralInfo.Id,
                Surname = userGeneralInfo.Surname,
                Name = userGeneralInfo.Name,
                CurrentPosition = versionId.HasValue ? userGeneralInfo.Versions.First(v => v.Id == versionId).VersionName : userGeneralInfo.Position?.Name,
                EmailAddressInCV = userGeneralInfo.EmailAddress,
                ImgPath = CommonUtils.FullFilePath(userGeneralInfo.AvatarPath),
                PhoneNumber = userGeneralInfo.PhoneNumber,
                Branch = userGeneralInfo.Branch?.Name,
                BranchId = userGeneralInfo.BranchId,
                CurrentPositionId = userGeneralInfo.PositionId,
                Address = userGeneralInfo.Address,
            };
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.MyProfile_User_Edit)]
        public async Task<UpdateUserGeneralInfoDto> SaveUserGeneralInfo([FromForm] UpdateUserGeneralInfoDto input)
        {
            var user = await WorkScope.GetAll<User, long>()
                                                 .Include(u => u.Position)
                                                 .Include(u => u.Branch)
                                                 .FirstOrDefaultAsync(u => u.Id == input.UserId);
            if (user == null)
            {
                throw new UserFriendlyException(ErrorCodes.NotFound.UserNotExist);
            }
            if (AbpSession.UserId.Value != input.UserId)
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.AccessOtherProfile);
            }
            user.Name = input.Name;
            user.Surname = input.Surname;
            user.PhoneNumber = input.PhoneNumber;
            user.EmailAddress = input.EmailAddressInCV;
            user.PositionId = input.CurrentPositionId;
            user.Address = input.Address;
            user.BranchId = input.BranchId;

            if (input.File != null)
            {
                user.AvatarPath = await _fileEmployeeService.UploadAvatar(input.File);
            }
            user.SetNormalizedNames();
            await WorkScope.GetRepo<User, long>().UpdateAsync(user);
            input.Path = CommonUtils.FullFilePath(user.AvatarPath);
            return input;
        }
        [AbpAuthorize(PermissionNames.MyProfile_Education_View)]
        public async Task<List<EmployeeEducationDto>> GetEducationInfo(long userId)
        {
            if ((AbpSession.UserId.Value != userId) && !await UserManager.IsGrantedAsync(AbpSession.UserId.Value, PermissionNames.Employee_ViewDetail))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.AccessOtherProfile);
            }

            var lstEdu = new List<EmployeeEducationDto>();
            var educations = WorkScope.GetAll<EmployeeEducation>()
                                      .Where(e => e.CVEmployeeId.Value == userId)
                                      .OrderBy(e => e.Order);
            foreach (var edu in educations)
            {
                var temp = new EmployeeEducationDto
                {
                    Id = edu.Id,
                    SchoolOrCenterName = edu.SchoolOrCenterName,
                    CvemployeeId = userId,
                    DegreeType = edu.DegreeType,
                    StartYear = edu.StartYear,
                    EndYear = edu.EndYear,
                    Description = edu.Description,
                    Major = edu.Major,
                    Order = edu.Order,
                };
                lstEdu.Add(temp);
            }
            return lstEdu;
        }
        [AbpAuthorize(PermissionNames.MyProfile_Education_Delete)]
        public async Task DeleteEducation(long id)
        {
            var edu = await WorkScope.GetAsync<EmployeeEducation>(id);
            if (edu == null)
            {
                throw new UserFriendlyException(ErrorCodes.NotFound.UserNotExist);
            }
            if (AbpSession.UserId.Value != edu.CVEmployeeId)
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.AccessOtherProfile);
            }
            await WorkScope.DeleteAsync<EmployeeEducation>(id);
        }
        [AbpAuthorize(PermissionNames.MyProfile_Education_Create, PermissionNames.MyProfile_Education_Edit)]
        public async Task<EmployeeEducationDto> SaveEducation(EmployeeEducationDto input)
        {
            if ((!WorkScope.GetAll<User, long>().Any(u => u.Id == input.CvemployeeId) || (!input.CvemployeeId.HasValue)))
            {
                throw new UserFriendlyException(ErrorCodes.NotFound.UserNotExist);
            }
            if (AbpSession.UserId.Value != input.CvemployeeId && !(await UserManager.IsGrantedAsync(AbpSession.UserId.Value, PermissionNames.Employee_ViewDetail)))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.AccessOtherProfile);
            }
            int startYear, endYear;
            if (!(int.TryParse(input.StartYear, out startYear) && int.TryParse(input.EndYear, out endYear)))
            {
                throw new UserFriendlyException(ErrorCodes.NotAcceptable.YearIsNotValid);
            }
            if (startYear > endYear)
            {
                throw new UserFriendlyException(ErrorCodes.NotAcceptable.YearOutOfRange);
            }

            if (input.Id <= 0)
            {
                var education = new EmployeeEducation
                {
                    CVEmployeeId = input.CvemployeeId,
                    SchoolOrCenterName = input.SchoolOrCenterName,
                    DegreeType = input.DegreeType,
                    Major = input.Major,
                    StartYear = input.StartYear,
                    EndYear = input.EndYear,
                    Description = input.Description,
                    Order = input.Order,
                };
                await WorkScope.InsertAsync(education);
                return input;
            }
            else
            {
                var education = await WorkScope.GetAsync<EmployeeEducation>(input.Id);
                education.CVEmployeeId = input.CvemployeeId;
                education.SchoolOrCenterName = input.SchoolOrCenterName;
                education.DegreeType = input.DegreeType;
                education.Major = input.Major;
                education.StartYear = input.StartYear;
                education.EndYear = input.EndYear;
                education.Description = input.Description;
                education.Order = input.Order;
                await WorkScope.UpdateAsync(education);
                return input;

            }
        }
        [AbpAuthorize(PermissionNames.MyProfile_WorkingExperience_Create, PermissionNames.MyProfile_WorkingExperience_Edit)]
        public async Task UpdateOrderWorkingExperience(List<OrderDto> input)
        {
            var exps = WorkScope.GetAll<EmployeeWorkingExperience>().Where(s => s.UserId == AbpSession.UserId);
            foreach (var item in input)
            {
                var exp = exps.FirstOrDefault(s => s.Id == item.Id);
                if (exp == default)
                {
                    continue;
                }
                exp.Order = item.Order;
                await WorkScope.UpdateAsync(exp);
            }
        }
        [AbpAuthorize(PermissionNames.MyProfile_PersonalAttribute_Create, PermissionNames.MyProfile_PersonalAttribute_Edit, PermissionNames.MyProfile_PersonalAttribute_Delete)]
        public async Task UpdatePersonalAttribute(PersonalAttributeDto input)
        {
            var user = await GetCurrentUserAsync();
            List<string> personalAttributes = new List<string>();
            string attributes;
            foreach (var item in input.PersonalAttributes)
            {
                if (item.Contains("•"))
                {
                    attributes = item.Replace("•", "");
                    personalAttributes.Add(attributes);
                }
                else if (item.Contains("●"))
                {
                    attributes = item.Replace("●", "");
                    personalAttributes.Add(attributes);
                }
                else
                {
                    personalAttributes.Add(item);
                }
            }
            user.PersonalAttribute = JsonConvert.SerializeObject(personalAttributes);
            await WorkScope.UpdateAsync(user);
        }
        [AbpAuthorize(PermissionNames.MyProfile_PersonalAttribute_View)]
        public async Task<PersonalAttributeDto> GetPersonalAttribute(long userId)
        {
            if ((AbpSession.UserId.Value != userId) && !await UserManager.IsGrantedAsync(AbpSession.UserId.Value, PermissionNames.Employee_ViewDetail))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.AccessOtherProfile);
            }
            var user = await UserManager.GetUserByIdAsync(userId);
            if (string.IsNullOrWhiteSpace(user.PersonalAttribute))
            {
                return new PersonalAttributeDto { PersonalAttributes = new List<string>() };
            }
            return new PersonalAttributeDto
            {
                PersonalAttributes = JsonConvert.DeserializeObject<List<string>>(user.PersonalAttribute)
            };
        }
        [AbpAuthorize(PermissionNames.MyProfile_Education_Create, PermissionNames.MyProfile_Education_Edit)]
        public async Task UpdateOrderEducation(List<OrderDto> input)
        {
            var lstEdu = WorkScope.GetAll<EmployeeEducation>().Where(e => e.CVEmployeeId == AbpSession.UserId);
            foreach (var item in input)
            {
                var edu = lstEdu.FirstOrDefault(s => s.Id == item.Id);
                if (AbpSession.UserId.Value != edu.CVEmployeeId)
                {
                    throw new UserFriendlyException(ErrorCodes.Forbidden.AccessOtherProfile);
                }
                edu.Order = item.Order;
                await WorkScope.UpdateAsync(edu);
            }
        }
    }
}
