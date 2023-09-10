using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Collections.Extensions;
using Abp.UI;
using Castle.Core.Internal;
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
using TalentV2.APIs.NccCVs.Employee.Dto;
using TalentV2.APIs.NccCVs.MyProfile.Dto;
using TalentV2.Authorization.Users;
using TalentV2.Constants.Enum.NccCVs;
using TalentV2.Entities.NccCVs;
using ProjectEntity = TalentV2.Entities.NccCVs.Project;
using TalentV2.Authorization;

namespace TalentV2.APIs.NccCVs.Employee
{
    [AbpAuthorize]
    public class EmployeeAppService : TalentV2AppServiceBase
    {
        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_ViewList)]
        public async Task<PagedResultDto<EmployeeDto>> GetAllEmployeePaging(GetEmployeeParam param)
        {
            var query = WorkScope.GetAll<User>()
                                .Include(u => u.Versions)
                                .WhereIf(!param.Name.IsNullOrEmpty(), u => u.Name.ToLower().Contains(param.Name.Trim().ToLower()) ||
                                                                        u.Surname.ToLower().Contains(param.Name.Trim().ToLower()) ||
                                                                          u.EmailAddress.ToLower().Contains(param.Name.Trim().ToLower()))
                                .WhereIf(param.BranchId.HasValue, u => u.BranchId == param.BranchId)
                                .WhereIf(param.PositionId.HasValue, u => u.PositionId == param.PositionId)
                                .WhereIf(param.PositionVersId.HasValue, u => u.Versions.Any(s => s.PositionId == param.PositionVersId))
                                .WhereIf(param.LanguageId.HasValue, u => u.Versions.Any(s => s.LanguageId == param.LanguageId))
                                .Select(u => new EmployeeDto
                                {
                                    UserId = u.Id,
                                    Name = u.Name,
                                    Surname = u.Surname,
                                    FullName = u.FullName,
                                    BranchId = u.BranchId,
                                    PositionId = u.PositionId,
                                });
            var totalCount = await query.CountAsync();
            var result = await query.OrderBy(p => p.Name).Skip(param.SkipCount).Take(param.MaxResultCount).ToListAsync();
            return new PagedResultDto<EmployeeDto>(totalCount, result);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.WorkingExperience_ViewAll)]
        public async Task<PagedResultDto<WorkingExperienceDto>> GetWorkingExperiencePaging(WorkingExperienceParam input)
        {
            var querry = WorkScope.GetAll<EmployeeWorkingExperience>().AsNoTracking()
                 .Where(u => input.IsIncludeVers || !u.VersionId.HasValue)
                 .Where(u => String.IsNullOrWhiteSpace(input.Technologies) || u.Technologies.ToLower().Trim().Contains(input.Technologies.ToLower().Trim()))
                 .Where(u => String.IsNullOrWhiteSpace(input.Positions) || u.Position.ToLower().Trim().Contains(input.Positions.ToLower().Trim()))
                 .Where(u => String.IsNullOrWhiteSpace(input.ProjectName) || u.Project.Name.ToLower().Trim().Contains(input.ProjectName.ToLower().Trim()))
                 .Where(u => String.IsNullOrWhiteSpace(input.EmployeeName) || (u.User.Name + " " + u.User.Surname).ToLower().Trim().Contains(input.EmployeeName.ToLower().Trim()))
                 .Select(w => new WorkingExperienceDto
                 {
                     Id = w.Id,
                     Position = w.Position,
                     ProjectDescription = w.ProjectDescription,
                     ProjectName = w.Project.Name,
                     ProjectId = w.ProjectId.HasValue ? w.ProjectId.Value : 0,
                     Responsibility = w.Responsibilities,
                     EndTime = w.EndTime,
                     StartTime = w.StartTime,
                     UserId = w.UserId,
                     UserName = w.User.FullName,
                     Order = w.Order,
                     Technologies = w.Technologies,
                     IsChecked = false,
                     VersionId = w.VersionId
                 });
            var totalCount = querry.Count();
            var result = querry.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            return new PagedResultDto<WorkingExperienceDto>(totalCount, result);
        }

        public async Task<object> GetEmployeeVersFilter(VersionFilterDto input)
        {
            long? userId = null;
            if (!(await UserManager.IsGrantedAsync(AbpSession.UserId.Value, PermissionNames.Employee_ViewList)))
            {
                userId = AbpSession.UserId;
            }

            return await WorkScope.GetAll<TalentV2.Entities.NccCVs.Versions>()
                                  .Include(v => v.Employee)
                                  .Include(v => v.Position)
                                  .Include(v => v.Language)
                                  .Where(v => String.IsNullOrEmpty(input.VersionName) || v.VersionName.ToLower().Trim().Contains(input.VersionName.ToLower().Trim()))
                                  .Where(v => !input.VersionLanguageId.HasValue || v.LanguageId == input.VersionLanguageId)
                                  .Where(v => !input.VersionPositionId.HasValue || v.PositionId == input.VersionPositionId)
                                  .Select(v => new
                                  {
                                      VersionId = v.Id,
                                      v.VersionName,
                                      v.EmployeeId,
                                      EmployeeName = v.Employee.Name,
                                      VersionLanguageId = v.LanguageId,
                                      VersionLanguage = v.Language.Name,
                                      VersionPositionId = v.PositionId,
                                      VersionPosition = v.Position.Name
                                  })
                                  .OrderBy(v => v.EmployeeName)
                                  .AsQueryable()
                                  .ToListAsync();
        }
    }
}