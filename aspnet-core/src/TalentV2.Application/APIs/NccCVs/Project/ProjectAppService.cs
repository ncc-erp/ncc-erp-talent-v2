using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.APIs.NccCVs.APIs.Project.Dto;
using TalentV2.APIs.NccCVs.Project.Dto;
using TalentV2.APIs.NccCVs.Technology.Dto;
using TalentV2.Authorization;
using TalentV2.Entities.NccCVs;

namespace TalentV2.APIs.NccCVs.Project
{
    [AbpAuthorize]
    public class ProjectAppService : TalentV2AppServiceBase
    {
        [HttpPost]
        [AbpAuthorize(PermissionNames.Project_Edit, PermissionNames.Project_Create)]
        public async Task<ProjectDto> Save(ProjectDto input)
        {
            if (input.Id <= 0)
            {
                input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<TalentV2.Entities.NccCVs.Project>(input));
            }
            else
            {
                var project = await WorkScope.GetAsync<TalentV2.Entities.NccCVs.Project>(input.Id);
                await WorkScope.UpdateAsync(ObjectMapper.Map<ProjectDto, TalentV2.Entities.NccCVs.Project>(input, project));
            }
            return input;
        }

        [HttpDelete]

        //[AbpAuthorize(PermissionNames.Project_Delete)]
        //public async Task Delete(long id)
        //{
        //    var isExist = await WorkScope.GetAll<EmployeeWorkingExperience>().AnyAsync(s => s.ProjectId == id);
        //    if (isExist)
        //    {
        //        throw new UserFriendlyException(string.Format("Project Id {0} already exist in working experiences", id));
        //    }
        //    await WorkScope.DeleteAsync<TalentV2.Entities.NccCVs.Project>(id);
        //}
        [HttpGet]
        [AbpAuthorize(PermissionNames.Project_ViewList,
                    PermissionNames.MyProfile_WorkingExperience_Create,
                    PermissionNames.MyProfile_WorkingExperience_Edit)]
        public async Task<List<ProjectDto>> GetAll(ProjectType? type, string name, string Technology, string TechVersion)
        {
            return (from s in WorkScope.GetAll<TalentV2.Entities.NccCVs.Project>()
                                        .WhereIf(!name.IsNullOrWhiteSpace(), s => !string.IsNullOrEmpty(s.Name) ? s.Name.Contains(name) : false)
                                        .WhereIf((type.HasValue && type != ProjectType.All) || (!type.HasValue), s => s.Type == type)
                                        .AsEnumerable()
                    select new ProjectDto
                    {
                        Id = s.Id,
                        Type = s.Type,
                        Description = s.Description,
                        Name = s.Name,
                        Technology = s.Technology,
                    }).ToList();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Project_ViewList)]
        public async Task<PagedResultDto<ProjectDto>> GetAllPaging(GetProjectParam input)
        {
            var query = WorkScope.GetAll<TalentV2.Entities.NccCVs.Project>()
                        .WhereIf(!input.Name.IsNullOrWhiteSpace(), s => string.IsNullOrWhiteSpace(s.Name) ? false : s.Name.ToLower().Contains(input.Name.Trim().ToLower()))
                        .WhereIf(input.Type.HasValue, s => s.Type == input.Type)
                        .AsEnumerable()
                        .Select(s => new ProjectDto
                        {
                            Id = s.Id,
                            Description = s.Description,
                            Name = s.Name,
                            Technology = s.Technology,
                            Type = s.Type
                        });

            var totalCount = query.Count();
            var result = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            return new PagedResultDto<ProjectDto>(totalCount, result);
        }

        [HttpGet]
        public async Task<ProjectDto> Get(long id)
        {
            return await WorkScope.GetAll<TalentV2.Entities.NccCVs.Project>().Where(s => s.Id == id)
                .Select(s => new ProjectDto
                {
                    Id = s.Id,
                    Type = s.Type,
                    Description = s.Description,
                    Name = s.Name,
                    Technology = s.Technology
                }).FirstOrDefaultAsync();
        }

        [HttpGet]
        public async Task<PagedResultDto<ReportOfProject>> ReportOfWorkedInProject(long ProjectId, int skipCount, int maxResultCount)
        {
            var query = WorkScope.GetAll<EmployeeWorkingExperience>().Where(s => s.ProjectId == ProjectId && s.VersionId != null)
               .OrderByDescending(s => s.CreationTime)
               .Select(s => new ReportOfProject
               {
                   UserId = s.UserId,
                   Name = $"{s.User.Surname} {s.User.Name}",
                   Position = s.Position,
                   StartDate = s.StartTime ?? default(DateTime),
                   EndDate = s.EndTime,
                   WorkingExpId = s.Id
               });
            return new PagedResultDto<ReportOfProject>
            {
                TotalCount = query.Count(),
                Items = query.Skip(skipCount).Take(maxResultCount).ToList()
            };
        }

        [HttpGet]
        public async Task<PagedResultDto<ReportOfProject>> ReportOfUsedProject(long ProjectId, int skipCount, int maxResultCount)
        {
            var query = WorkScope.GetAll<EmployeeWorkingExperience>().Where(s => s.ProjectId == ProjectId && s.VersionId != null)
               .OrderByDescending(s => s.CreationTime)
               .Select(s => new ReportOfProject
               {
                   UserId = s.UserId,
                   Name = $"{s.User.Surname} {s.User.Name}",
                   Position = s.Position,
                   StartDate = s.StartTime ?? default(DateTime),
                   EndDate = s.EndTime,
                   WorkingExpId = s.Id
               }).AsEnumerable().GroupBy(s => s.UserId)
               .Select(s => new ReportOfProject
               {
                   UserId = s.Key,
                   Name = s.FirstOrDefault().Name,
                   Position = s.FirstOrDefault().Position,
                   StartDate = s.FirstOrDefault().StartDate,
                   EndDate = s.FirstOrDefault().EndDate,
                   WorkingExpId = s.FirstOrDefault().WorkingExpId
               });
            return new PagedResultDto<ReportOfProject>
            {
                TotalCount = query.Count(),
                Items = query.Skip(skipCount).Take(maxResultCount).ToList()
            };
        }

        [HttpPost]
        public async Task AddTechToProject(ChangeProjectAndTechnologyDto input)
        {
            var oldTechnology = await WorkScope.GetAll<ProjectTechnology>().Where(s => s.ProjectId == input.ProjectId).Select(s => s.TechnologyId).ToListAsync();
            var listToAdd = input.TechnologyId.Except(oldTechnology).ToList();
            var ListToAdd = new List<ProjectTechnology>();
            foreach (var i in listToAdd)
            {
                ListToAdd.Add(new ProjectTechnology { ProjectId = input.ProjectId, TechnologyId = i, Status = true });
            }
            await WorkScope.InsertRangeAsync(ListToAdd);
            /* var DataToDelete = WorkScope.GetAll<ProjectTechnology>().Where(s => s.ProjectId == input.ProjectId && listToRemove.Contains(s.TechnologyId));
             foreach(var i in DataToDelete)
             {
                 await WorkScope.DeleteAsync(i);
             }*/
        }

        [HttpGet]
        public async Task<PagedResultDto<TechnologyDto>> ProjectAndTechPaging(long ProjectId, string TechName, int SkipCount, int MaxResultCount)
        {
            var query = WorkScope.GetRepo<ProjectTechnology>().GetAllIncluding(s => s.Technologies)
                    .Where(s => s.ProjectId == ProjectId)
                    .WhereIf(!TechName.IsNullOrWhiteSpace(), s => s.Technologies.TechnologyName.Contains(TechName, StringComparison.OrdinalIgnoreCase))
                    .AsEnumerable()
                    .Select(s => new TechnologyDto
                    {
                        Id = s.TechnologyId,
                        TechName = s.Technologies == null ? null : s.Technologies.TechnologyName,
                        Version = s.Technologies == null ? null : s.Technologies.Version,
                        isChecked = s.Status
                    });
            return new PagedResultDto<TechnologyDto>
            {
                TotalCount = query.Count(),
                Items = query.Skip(SkipCount).Take(MaxResultCount).ToList(),
            };
        }

        [HttpGet]
        public async Task ChangeStatusOfTech(long ProjectId, long TechId, Boolean Status)
        {
            var ProjectTech = await WorkScope.GetAll<ProjectTechnology>().Where(s => s.ProjectId == ProjectId && TechId == s.TechnologyId).FirstOrDefaultAsync();
            if (ProjectTech.Status != Status)
            {
                ProjectTech.Status = Status;
                await WorkScope.UpdateAsync(ProjectTech);
            }
        }

        [HttpGet]
        public async Task<PagedResultDto<GetAllTechnologiesDto>> GetAllTechNotInProject(long ProjectId, string TechName, int skipCount, int maxResultCount)
        {
            return await Task.Run(() =>
            {
                var TechInProject = WorkScope.GetAll<ProjectTechnology>().Where(s => s.ProjectId == ProjectId).Select(s => s.TechnologyId);
                var Query = WorkScope.GetAll<TalentV2.Entities.NccCVs.Technology>()
                    .Where(s => !TechInProject.Contains(s.Id))
                    .WhereIf(!TechName.IsNullOrWhiteSpace(), s => s.TechnologyName.Contains(TechName))
                    .Select(s => new GetAllTechnologiesDto
                    {
                        Id = s.Id,
                        Technology = s.TechnologyName,
                        Version = s.Version
                    });
                return new PagedResultDto<GetAllTechnologiesDto>
                {
                    TotalCount = Query.Count(),
                    Items = Query.Skip(skipCount).Take(maxResultCount).ToList()
                };
            });
        }

        [HttpGet]
        public async Task RemoveProjectAndTech(long ProjectId, long TechId)
        {
            var data = WorkScope.GetAll<ProjectTechnology>().Where(s => s.ProjectId == ProjectId && s.TechnologyId == TechId);
            foreach (var i in data)
            {
                await WorkScope.DeleteAsync(i);
            }
        }
    }
}