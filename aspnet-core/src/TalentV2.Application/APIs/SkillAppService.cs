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
    public class SkillAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public SkillAppService(ICategoryManager categoryManager) 
        {
            _categoryManager = categoryManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Skills_ViewList)]
        public async Task<GridResult<CategoryDto>> GetAllPaging(GridParam param)
        {
            var query = _categoryManager.IQGetAllSkills();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Skills_Create)]
        public async Task<CategoryDto> Create(SkillDto input)
        {
            return await _categoryManager.CreateSkill(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_Skills_Edit)]
        public async Task<CategoryDto> Update(SkillDto input)
        {
            return await _categoryManager.UpdateSkill(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_Skills_Delete)]
        public async Task<string> Delete(long Id)
        {
            await _categoryManager.DeleteSkill(Id);
            return "Deleted Successfully!";
        }
        [HttpGet]
        public async Task<List<CategoryDto>> GetAll()
        {
            return await _categoryManager
                .IQGetAllSkills()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
