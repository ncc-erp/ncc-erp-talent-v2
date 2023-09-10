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
    public class PositionAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public PositionAppService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_JobPositions_ViewList)]
        public async Task<GridResult<PositionDto>> GetAllPaging(GridParam param)
        {
            var query = _categoryManager
                .IQGetAllPosition();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_JobPositions_Create)]
        public async Task<PositionDto> Create(PositionDto input)
        {
            return await _categoryManager.CreatePosition(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_JobPositions_Edit)]
        public async Task<PositionDto> Update(PositionDto input)
        {
            return await _categoryManager.UpdatePosition(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_JobPositions_Delete)]
        public async Task<string> Delete(long Id)
        {
            await _categoryManager.DeletePosition(Id);
            return "Deleted Successfully";
        }
        [HttpGet]
        public async Task<List<PositionDto>> GetAll()
        {
            return await _categoryManager
                .IQGetAllPosition()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
