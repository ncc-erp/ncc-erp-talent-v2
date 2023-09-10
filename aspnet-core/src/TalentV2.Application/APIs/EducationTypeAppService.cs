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
    public class EducationTypeAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public EducationTypeAppService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_EducationTypes_ViewList)]
        public async Task<GridResult<CategoryDto>> GetAllPaging(GridParam param)
        {
            var query = _categoryManager.IQGetAllEducationType();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_EducationTypes_Create)]
        public async Task<CategoryDto> Create(EducationTypeDto input)
        {
            return await _categoryManager.CreateEducationType(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_EducationTypes_Edit)]
        public async Task<CategoryDto> Update(EducationTypeDto input)
        {
            return await _categoryManager.UpdateEducationType(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_EducationTypes_Delete)]
        public async Task<string> Delete(long Id)
        {
            await _categoryManager.DeleteEducationType(Id);
            return "Deleted Successfully";
        }
        [HttpGet]
        public async Task<List<CategoryDto>> GetAll()
        {
            return await _categoryManager
                .IQGetAllEducationType()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
