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
    public class BranchAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public BranchAppService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Branches_ViewList)]
        public async Task<GridResult<BranchDto>> GetAllPaging(GridParam param)
        {
            var query = _categoryManager
                .IQGetAllBranches();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Branches_Create)]
        public async Task<BranchDto> Create(CreateBranchDto input)
        {
            return await _categoryManager.CreateBranch(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_Branches_Edit)]
        public async Task<BranchDto> Update(UpdateBranchDto input)
        {
            return await _categoryManager.UpdateBranch(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_Branches_Delete)]
        public async Task Delete(long Id)
        {
            await _categoryManager.DeleteBranch(Id);
        }
        [HttpGet]
        public async Task<List<BranchDto>> GetAll()
        {
            return await _categoryManager
                .IQGetAllBranches()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
