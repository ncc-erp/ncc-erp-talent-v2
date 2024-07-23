using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using TalentV2.Authorization;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;
using TalentV2.Editions;
using TalentV2.MultiTenancy.Dto;
using Microsoft.AspNetCore.Identity;
using TalentV2.Entities;
using System.Collections.Generic;
using TalentV2.NccCore;
using TalentV2.Constants.Enum;
using System;
using TalentV2.Constants.Dictionary;
using TalentV2.Notifications.Templates;
using Abp.Authorization.Roles;
using NccCore.Extension;

namespace TalentV2.MultiTenancy
{
    [AbpAuthorize(PermissionNames.Pages_Tenants)]
    public class TenantAppService : AsyncCrudAppService<Tenant, TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>, ITenantAppService
    {
        private readonly TenantManager _tenantManager;
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IWorkScope _workScope;

        public TenantAppService(
            IRepository<Tenant, int> repository,
            TenantManager tenantManager,
            EditionManager editionManager,
            UserManager userManager,
            RoleManager roleManager,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            IWorkScope workScope)
            : base(repository)
        {
            _tenantManager = tenantManager;
            _editionManager = editionManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _workScope = workScope;
        }

        public override async Task<TenantDto> CreateAsync(CreateTenantDto input)
        {
            CheckCreatePermission();

            // Create tenant
            var tenant = ObjectMapper.Map<Tenant>(input);
            tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
                ? null
                : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

            var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                tenant.EditionId = defaultEdition.Id;
            }

            await _tenantManager.CreateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync(); // To get new tenant's id.

            // Create tenant database
            _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            // We are working entities of new tenant, so changing tenant filter
            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                // Create static roles for new tenant
                CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));

                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                // Grant all permissions to admin role
                var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                await _roleManager.GrantAllPermissionsAsync(adminRole);

                // Create admin user for the tenant
                var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
                await _userManager.InitializeOptionsAsync(tenant.Id);
                CheckErrors(await _userManager.CreateAsync(adminUser, User.DefaultPassword));
                await CurrentUnitOfWork.SaveChangesAsync(); // To get admin user's id

                // Assign admin user to role!
                CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));

                await CreateMailTemplate(tenant.Id);
                await CurrentUnitOfWork.SaveChangesAsync();

                await CreateRoleAndAddPermission(tenant.Id);
                await CurrentUnitOfWork.SaveChangesAsync();

            }

            return MapToEntityDto(tenant);
        }

        protected override IQueryable<Tenant> CreateFilteredQuery(PagedTenantResultRequestDto input)
        {
            var keyword = input.Keyword.EmptyIfNull().Trim().ToLower();
            return Repository.GetAll()
                .WhereIf(!keyword.IsNullOrWhiteSpace(), x => x.TenancyName.ToLower().Contains(keyword) || x.Name.ToLower().Contains(keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected override void MapToEntity(TenantDto updateInput, Tenant entity)
        {
            // Manually mapped since TenantDto contains non-editable properties too.
            entity.Name = updateInput.Name;
            entity.TenancyName = updateInput.TenancyName;
            entity.IsActive = updateInput.IsActive;
        }

        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();

            var tenant = await _tenantManager.GetByIdAsync(input.Id);
            await _tenantManager.DeleteAsync(tenant);
        }

        private void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
        private async Task CreateMailTemplate(int? tenantId)
        {
            var mailTemplates = new List<EmailTemplate>();
            Enum.GetValues(typeof(MailFuncEnum))
                .Cast<MailFuncEnum>()
                .ToList()
                .ForEach(e =>
                {
                    var mailSeeds = DictionaryHelper.SeedMailDic[e];
                    if (mailSeeds != null && mailSeeds.Count > 0)
                    {
                        foreach (var mail in mailSeeds)
                        {
                            mailTemplates.Add(
                                new EmailTemplate
                                {
                                    Subject = mail.Subject,
                                    Name = mail.Name,
                                    BodyMessage = TemplateHelper.ContentEmailTemplate(e),
                                    Description = mail.Description,
                                    Type = e,
                                    Version = mail.Version,
                                    TenantId = tenantId
                                }
                            );
                        }
                    }
                });
            await _workScope.InsertRangeAsync(mailTemplates);
        }

        private async Task CreateRoleAndAddPermission(int? tenantId)
        {
            var roleSeeds = new List<string>() { StaticRoleNames.Tenants.Sale,
                                                StaticRoleNames.Tenants.BasicUser,
                                                StaticRoleNames.Tenants.Recruitment,
                                                StaticRoleNames.Tenants.Interviewer };

            foreach (var roleSeed in roleSeeds)
            {
                var input = new Role
                {
                    TenantId = tenantId,
                    Name = roleSeed,
                    DisplayName = roleSeed,
                    IsStatic = false
                };

                var role = ObjectMapper.Map<Role>(input);
                role.SetNormalizedName();

                CheckErrors(await _roleManager.CreateAsync(role));

                var grantedPermissionsByRole = GrantPermissionRoles.PermissionRoles
                                                .Where(x => x.Key == roleSeed)
                                                .FirstOrDefault()
                                                .Value;
                if (grantedPermissionsByRole != null)
                {
                    var grantedPermissions = PermissionManager
                        .GetAllPermissions()
                        .Where(p => grantedPermissionsByRole.Contains(p.Name))
                        .ToList();
                    await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
                }
            }
        }
    }
}

