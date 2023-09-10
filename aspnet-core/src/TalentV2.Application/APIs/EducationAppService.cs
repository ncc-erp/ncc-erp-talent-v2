using Abp.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.DomainServices.Categories;
using TalentV2.DomainServices.Categories.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class EducationAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public EducationAppService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Educations_ViewList)]
        public async Task<GridResult<EducationDto>> GetAllPaging(GridParam param)
        {
            var query = _categoryManager
                .IQGetAllEducation();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Educations_Create)]
        public async Task<EducationDto> Create(CreateUpdateEducationDto input)
        {
            return await _categoryManager.CreateEducation(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_Educations_Edit)]
        public async Task<EducationDto> Update(CreateUpdateEducationDto inputEducation)
        {
            return await _categoryManager.UpdateEducation(inputEducation);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_Educations_Delete)]
        public async Task<string> Delete(long Id)
        {
            await _categoryManager.DeleteEducation(Id);
            return "Deleted Successfully!";
        }
        [HttpGet]
        public async Task<List<EducationDto>> GetAll()
        {
            return await _categoryManager.IQGetAllEducation()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
