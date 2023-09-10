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
    public class CapabilityAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public CapabilityAppService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Capabilities_ViewList)]
        public async Task<GridResult<CapabilityDto>> GetAllPaging(GridParam param)
        {
            var query = _categoryManager
                .IQGetAllCapability();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Capabilities_Create)]
        public async Task<CapabilityDto> Create(CapabilityDto input)
        {
            return await _categoryManager.CreateCapability(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_Capabilities_Edit)]
        public async Task<CapabilityDto> Update(CapabilityDto input)
        {
            return await _categoryManager.UpdateCapability(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_Capabilities_Delete)]
        public async Task<string> Delete(long Id)
        {
            await _categoryManager.DeleteCapability(Id);
            return "Deleted Successfully!";
        }
        [HttpGet]
        public async Task<List<CapabilityDto>> GetAll()
        {
            return await _categoryManager
                .IQGetAllCapability()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
