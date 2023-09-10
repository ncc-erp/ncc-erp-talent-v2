using Abp.Authorization;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.APIs.NccCVs.Versions.Dto;
using TalentV2.Authorization;
using TalentV2.Constants.Enum.NccCVs;
using TalentV2.Entities.NccCVs;

namespace TalentV2.APIs.NccCVs.Versions
{
    [AbpAuthorize]
    public class VersionAppService : TalentV2AppServiceBase
    {
        public async Task<VersionDto> CreateVersion(VersionDto input)
        {
            if (input.EmployeeId != AbpSession.UserId && !(await IsGrantedAsync(PermissionNames.Employee_ViewDetail)))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.EditOtherProfile);
            }

            if (await WorkScope.GetAll<TalentV2.Entities.NccCVs.Versions>().AnyAsync(s => s.VersionName.ToLower().Trim() == input.VersionName.Trim().ToLower()
                                                                && s.EmployeeId == input.EmployeeId))
            {
                throw new UserFriendlyException(ErrorCodes.Conflict.VersionName);
            }

            var version = ObjectMapper.Map<TalentV2.Entities.NccCVs.Versions>(input);

            input.Id = await WorkScope.InsertAndGetIdAsync(version);
            //insert EmployeeWorkingExperiences
            var workingExperienceDefault = await WorkScope.GetAll<EmployeeWorkingExperience>().Where(s => s.UserId == input.EmployeeId && s.VersionId == null).ToListAsync();
            foreach (var w in workingExperienceDefault)
            {
                var working = new EmployeeWorkingExperience
                {
                    UserId = w.UserId,
                    VersionId = input.Id,
                    Position = w.Position,
                    ProjectDescription = w.ProjectDescription,
                    Responsibilities = w.Responsibilities,
                    ProjectId = w.ProjectId,
                    StartTime = w.StartTime,
                    EndTime = w.EndTime,
                    Technologies = w.Technologies,
                    Order = w.Order
                };
                await WorkScope.InsertAsync(working);
            }
            return input;
        }
        
        public async Task DeleteVersion(long id)
        {
            var isExist = await WorkScope.GetAll<TalentV2.Entities.NccCVs.Versions>().AnyAsync(s => s.Id == id);
            if (!isExist)
            {
                throw new UserFriendlyException(string.Format("Version Id = {0} isn't exist", id));
            }
            await WorkScope.DeleteAsync<TalentV2.Entities.NccCVs.Versions>(id);
            var experiences = await WorkScope.GetAll<EmployeeWorkingExperience>().Where(s => s.VersionId == id).Select(s => s.Id).ToListAsync();
            if (experiences != null)
            {
                foreach (var e in experiences)
                {
                    await WorkScope.DeleteAsync<EmployeeWorkingExperience>(e);
                }
            }
        }

        public async Task<object> GetAllVersion([Required] long employeeId)
        {
            if (employeeId != AbpSession.UserId && !(await IsGrantedAsync(PermissionNames.Employee_ViewDetail)))
            {
                throw new UserFriendlyException(ErrorCodes.Forbidden.ViewOtherProfile);
            }
            return await WorkScope.GetAll<TalentV2.Entities.NccCVs.Versions>()
                            .Include(s => s.Language)
                            .Include(s => s.Position)
                            .Where(s => s.EmployeeId == employeeId)
                            .Select(s => new
                            {
                                VersionId = s.Id,
                                s.VersionName,
                                PositionName = s.Position.Name,
                                LanguageName = s.Language.Name
                            }).ToListAsync();
        }
    }
}
