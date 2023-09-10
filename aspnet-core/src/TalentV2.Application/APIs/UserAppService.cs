using TalentV2.Authorization.Users;
using Microsoft.AspNetCore.Mvc;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization;
using TalentV2.DomainServices.Users.Dtos;
using TalentV2.DomainServices.Users;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class UserAppService : TalentV2AppServiceBase
    {
        private readonly IUserManager _userManager;
        public UserAppService(IUserManager userManager) 
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<GridResult<AllUserPagingDto>> GetAllPaging(GridParam gridParam)
        {
            var searchTerm = gridParam.SearchText.EmptyIfNull().Trim().ToLower();
            var qallUsersPaging = _userManager.IQGetAllUserPaging()
                .Where(x => (x.Surname + " " + x.Name).Trim().ToLower().Contains(searchTerm) || x.EmailAddress.Trim().ToLower().Contains(searchTerm))
                .OrderByDescending(s => s.LastModifiedTime ?? DateTime.MinValue)
                .ThenByDescending(s => s.CreationTime);
            gridParam.SearchText = string.Empty;
            return await qallUsersPaging.GetGridResult(qallUsersPaging, gridParam);

        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var allUsers = await dbContext.Users.AsQueryable().Select(x => x.Id).ToListAsync();
            return new OkObjectResult(allUsers);
        }
    }
}
