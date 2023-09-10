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
    public class CVSourceAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public CVSourceAppService(ICategoryManager categoryManager) 
        {
            _categoryManager = categoryManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CVSources_ViewList)]
        public async Task<GridResult<CVSourceDto>> GetAllPaging(GridParam param)
        {
            var query = _categoryManager
                .IQGetAllCVSources();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CVSources_Create)]
        public async Task<CVSourceDto> Create(CVSourceDto input)
        {
            return await _categoryManager.CreateCVSource(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_CVSources_Edit)]
        public async Task<CVSourceDto> Update(CVSourceDto input)
        {
            return await _categoryManager.UpdateCVSource(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_CVSources_Delete)]
        public async Task<string> Delete(long Id)
        {
            await _categoryManager.DeleteCVSource(Id);
            return "Deleted Successfully!";
        }
        [HttpGet]
        public async Task<List<CVSourceDto>> GetAll()
        {
            return await _categoryManager
                .IQGetAllCVSources()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
