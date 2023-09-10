using Abp.Authorization;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;
using TalentV2.DomainServices.Categories;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.DomainServices.Posts.Dtos;
using TalentV2.DomainServices.Users.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class PostAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public PostAppService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Posts_ViewList)]
        public async Task<GridResult<PostDto>> GetAllPaging(PostFilterPaging paramFilters)
        {
            var query = _categoryManager
            .IQGetAllPosts()
            .WhereIf(paramFilters.FromDate.HasValue, q => q.PostCreationTime.Date >= paramFilters.FromDate.Value.Date)
            .WhereIf(paramFilters.ToDate.HasValue, q => q.PostCreationTime.Date <= paramFilters.ToDate.Value.Date);

            return await query.GetGridResult(query, paramFilters);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Posts_Create)]
        public async Task<PostDto> Create(CreatePostDto input)
        {
            return await _categoryManager.CreatePost(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_Posts_Edit)]
        public async Task<PostDto> Update(UpdatePostDto input)
        {
            return await _categoryManager.UpdatePost(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_Posts_Delete)]
        public async Task<string> Delete(long Id)
        {
            await _categoryManager.DeletePost(Id);
            return "Deleted Successfully";
        }
        [HttpGet]
        public async Task<List<PostDto>> GetAll()
        {
            return await _categoryManager
                .IQGetAllPosts()
                .OrderBy(x => x.PostName)
                .ToListAsync();
        }
        [HttpGet]
        public async Task<List<UserReferenceDto>> GetAllRecruitmentUser()
        {
            return await _categoryManager.GetAllRecruitmentUser();
        }
    }
}
