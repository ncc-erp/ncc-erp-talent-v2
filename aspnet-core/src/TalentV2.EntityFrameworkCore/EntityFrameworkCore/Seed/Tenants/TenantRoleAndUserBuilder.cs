using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using TalentV2.Authorization;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;
using TalentV2.SeedData;
using Abp.Dependency;
using TalentV2.Utils;
using Abp.UI;
using Microsoft.Extensions.Configuration;
using TalentV2.Entities;
using TalentV2.Constants.Enum;
using System.Collections.Generic;
using System;
using TalentV2.Constants.Dictionary;
using TalentV2.Notifications.Templates;
using TalentV2.EntityFrameworkCore.Seed.Emails;
using System.Collections.Generic;

namespace TalentV2.EntityFrameworkCore.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly TalentV2DbContext _context;
        private readonly int _tenantId;

        public TenantRoleAndUserBuilder(TalentV2DbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            // Admin role

            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            // Grant all permissions to admin role

            var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == _tenantId && p.RoleId == adminRole.Id)
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(new TalentV2AuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
                            !grantedPermissions.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                _context.Permissions.AddRange(
                    permissions.Select(permission => new RolePermissionSetting
                    {
                        TenantId = _tenantId,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRole.Id
                    })
                );
                _context.SaveChanges();
            }

            CreateRoleAndAddPermission(_tenantId);
            // Admin user
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection(SeedDataConfig.SeedDataConfigKey);

            var defaultTenantAdminMail = config[SeedDataConfig.DefaultTenantAdminEmail];
            if(string.IsNullOrEmpty(defaultTenantAdminMail))
                throw new UserFriendlyException("Not found Tenant Admin Mail Config Seed Data");

            var defaultTenantAdminName = config[SeedDataConfig.DefaultTenantName];
            if(string.IsNullOrEmpty(defaultTenantAdminName))
                throw new UserFriendlyException("Not found Tenant Admin Name Config Seed Data");

            var isDefaultPasswordComplex = config[SeedDataConfig.IsDefaultPasswordComplex];
            if(isDefaultPasswordComplex == null)
                throw new UserFriendlyException("Not found Password Complex Config Seed Data");

            string password = PasswordUtils.GeneratePassword(12,bool.Parse(isDefaultPasswordComplex));

            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, defaultTenantAdminMail);
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, password);
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                // Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();
            }
            new DefaultEmailSettingsCreator(_context, adminRole.TenantId).Create();
        }

        private void CreateRoleAndAddPermission(int? tenantId)
        {
            var roleSeeds = new List<string>() { StaticRoleNames.Tenants.Sale,
                                                StaticRoleNames.Tenants.BasicUser,
                                                StaticRoleNames.Tenants.Recruitment,
                                                StaticRoleNames.Tenants.Interviewer };
            foreach (var roleSeed in roleSeeds)
            {
                var role = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == tenantId && r.Name == roleSeed);
                if (role == null)
                {
                    role = _context.Roles.Add(new Role(tenantId, roleSeed, roleSeed) { IsStatic = false }).Entity;
                    _context.SaveChanges();

                    var grantedPermissionsByRole = GrantPermissionRoles.PermissionRoles
                                                   .Where(x => x.Key == roleSeed)
                                                   .FirstOrDefault()
                                                   .Value;

                    if (grantedPermissionsByRole != null) {
                        var permissions = PermissionFinder.GetAllPermissions(new TalentV2AuthorizationProvider())
                                            .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
                                                grantedPermissionsByRole.Contains(p.Name))
                                            .ToList();

                        if (permissions.Any())
                        {
                            _context.Permissions.AddRange(
                                permissions.Select(permission => new RolePermissionSetting
                                {
                                    TenantId = tenantId,
                                    Name = permission.Name,
                                    IsGranted = true,
                                    RoleId = role.Id
                                })
                            );
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
