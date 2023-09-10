using Abp.Authorization;
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
    public class SubPostisionAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public SubPostisionAppService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_SubPositions_ViewList)]
        public async Task<GridResult<SubPositionDto>> GetAllPaging(GridParam param)
        {
            var query = _categoryManager
                .IQGetAllSubPosition();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_SubPositions_Create)]
        public async Task<SubPositionDto> Create(SubPositionDto input)
        {
            return await _categoryManager.CreateSubPosition(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_SubPositions_Edit)]
        public async Task<SubPositionDto> Update(SubPositionDto input)
        {
            return await _categoryManager.UpdateSubPosition(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_SubPositions_Delete)]
        public async Task<string> Delete(long Id)
        {
            await _categoryManager.DeleteSubPosition(Id);
            return "Deleted Successfully";
        }
        [HttpGet]
        public async Task<List<SubPositionDto>> GetAll()
        {
            return await _categoryManager
                .IQGetAllSubPosition()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
