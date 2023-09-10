using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using TalentV2.Authorization;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;
using TalentV2.Roles.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static TalentV2.Authorization.GrantPermissionRoles;
using TalentV2.NccCore;
using Abp.Authorization.Users;
using Microsoft.AspNetCore.Mvc;
using NccCore.Extension;
using TalentV2.DomainServices.Users.Dtos;

namespace TalentV2.Roles
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class RoleAppService : AsyncCrudAppService<Role, RoleDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>, IRoleAppService
    {
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IWorkScope _ws;

        public RoleAppService(IRepository<Role> repository, RoleManager roleManager, UserManager userManager, IWorkScope ws)
            : base(repository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _ws = ws;
        }

        public override async Task<RoleDto> CreateAsync(CreateRoleDto input)
        {
            CheckCreatePermission();

            var role = ObjectMapper.Map<Role>(input);
            role.SetNormalizedName();

            CheckErrors(await _roleManager.CreateAsync(role));

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissions.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return MapToEntityDto(role);
        }

        public async Task<ListResultDto<RoleListDto>> GetRolesAsync(GetRolesInput input)
        {
            var roles = await _roleManager
                .Roles
                .WhereIf(
                    !input.Permission.IsNullOrWhiteSpace(),
                    r => r.Permissions.Any(rp => rp.Name == input.Permission && rp.IsGranted)
                )
                .ToListAsync();

            return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
        }

        public override async Task<RoleDto> UpdateAsync(RoleDto input)
        {
            CheckUpdatePermission();

            var role = await _roleManager.GetRoleByIdAsync(input.Id);

            ObjectMapper.Map(input, role);

            CheckErrors(await _roleManager.UpdateAsync(role));

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissions.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return MapToEntityDto(role);
        }

        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();

            var role = await _roleManager.FindByIdAsync(input.Id.ToString());
            var users = await _userManager.GetUsersInRoleAsync(role.NormalizedName);

            foreach (var user in users)
            {
                CheckErrors(await _userManager.RemoveFromRoleAsync(user, role.NormalizedName));
            }

            CheckErrors(await _roleManager.DeleteAsync(role));
        }

        public List<SystemPermission> GetAllPermissions()
        {
            var permissions = SystemPermission.TreePermissions;

            return permissions;
        }

        protected override IQueryable<Role> CreateFilteredQuery(PagedRoleResultRequestDto input)
        {
            var keyword = input.Keyword.EmptyIfNull().Trim().ToLower();
            return Repository.GetAllIncluding(x => x.Permissions)
                .WhereIf(!keyword.IsNullOrEmpty(), x => x.Name.ToLower().Contains(keyword)
                || x.DisplayName.ToLower().Contains(keyword)
                || x.Description.ToLower().Contains(keyword));
        }

        protected override async Task<Role> GetEntityByIdAsync(int id)
        {
            return await Repository.GetAllIncluding(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == id);
        }

        protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, PagedRoleResultRequestDto input)
        {
            return query.OrderBy(r => r.DisplayName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<GetRoleForEditOutput> GetRoleForEdit(EntityDto input)
        {

            var permissions = SystemPermission.TreePermissions;
            var role = await _roleManager.GetRoleByIdAsync(input.Id);
            var grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
            var roleEditDto = ObjectMapper.Map<RoleEditDto>(role);

            return new GetRoleForEditOutput
            {
                Role = roleEditDto,
                Permissions = permissions,
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        public async Task<List<UserRoleInfoDto>> GetUserInRole(long id)
        {
            var dicUserRoleIds = _ws.GetAll<UserRole>()
                .Where(q => q.RoleId == id)
                .Select(s => new { s.UserId, s.Id })
                .AsEnumerable()
                .DistinctBy(s => s.UserId)
                .ToDictionary(s => s.UserId, s => s.Id);

            var userIds = dicUserRoleIds.Keys.ToList();

            var users = _ws.GetAll<User>()
                .Where(q => userIds.Contains(q.Id))
                .Include(q => q.Branch)
                .AsNoTracking()
                .AsEnumerable()
                .Select(s => new UserRoleInfoDto
                {
                    UserRoleId = dicUserRoleIds[s.Id],
                    AvatarPath = s.AvatarPath,
                    FullName = s.FullName,
                    UserId = s.Id,
                    Name = s.Name,
                    Surname = s.Surname,
                    Phone = s.PhoneNumber,
                    Email = s.EmailAddress,
                    BranchName = s.Branch?.Name
                }).ToList();
            return users;
        }
        public async Task<List<UserRoleInfoDto>> GetUserNotInRole(long id)
        {
            var userIds = await _ws.GetAll<UserRole>()
                .Where(q => q.RoleId == id)
                .Select(s => s.UserId)
                .ToListAsync();
            var users = await _ws.GetAll<User>()
                .Where(q => !userIds.Contains(q.Id))
                .Select(s => new UserRoleInfoDto
                {
                    AvatarPath = s.AvatarPath,
                    FullName = s.FullName,
                    UserId = s.Id,
                    Name = s.Name,
                    Surname = s.Surname,
                    Phone = s.PhoneNumber,
                    Email = s.EmailAddress,
                    BranchName = s.Branch.Name
                }).ToListAsync();
            return users;
        }

        public async Task<string> CreateUserRole(UserRoleDto input)
        {
            foreach(var userId in input.UserIds)
            {
                await _ws.InsertAsync(new UserRole
                {
                    RoleId = input.RoleId,
                    UserId = userId,
                });
            }
            return "Created User Role Successfully";
        }
        [HttpPost]
        public async Task<string> DeleteUserRole(DeleteUserRoleDto input)
        {
            foreach(var userRoleId in input.UserRoleIds)
            {
                await _ws.DeleteAsync<UserRole>(userRoleId);
            }
            return "Delete User Role Successfully";
        }

    }
}

