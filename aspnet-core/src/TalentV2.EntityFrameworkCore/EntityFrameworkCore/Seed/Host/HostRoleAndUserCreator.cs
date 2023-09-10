using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using TalentV2.Authorization;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TalentV2.SeedData;
using Abp.Dependency;
using Abp.UI;
using Microsoft.Extensions.Configuration;
using System.IO;
using TalentV2.Utils;
using System.Collections.Generic;

namespace TalentV2.EntityFrameworkCore.Seed.Host
{
    public class HostRoleAndUserCreator
    {
        private readonly TalentV2DbContext _context;

        public HostRoleAndUserCreator(TalentV2DbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateHostRoleAndUsers();
        }

        private void CreateHostRoleAndUsers()
        {
            // Admin role for host

            var adminRoleForHost = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Host.Admin);
            if (adminRoleForHost == null)
            {
                adminRoleForHost = _context.Roles.Add(new Role(null, StaticRoleNames.Host.Admin, StaticRoleNames.Host.Admin) { IsStatic = true, IsDefault = true }).Entity;
                _context.SaveChanges();
            }

            // Grant all permissions to admin role for host

            var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == null && p.RoleId == adminRoleForHost.Id)
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(new TalentV2AuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host) &&
                            !grantedPermissions.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                _context.Permissions.AddRange(
                    permissions.Select(permission => new RolePermissionSetting
                    {
                        TenantId = null,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRoleForHost.Id
                    })
                );
                _context.SaveChanges();
            }

            CreateRoleAndAddPermission(null);
            // Admin user for host
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection(SeedDataConfig.SeedDataConfigKey);

            var emailHost = config[SeedDataConfig.AdminHostEmail];
            if (string.IsNullOrEmpty(emailHost))
                throw new UserFriendlyException("Not found Admin Host Mail Config Seed Data");

            var isDefaultPasswordComplex = config[SeedDataConfig.IsDefaultPasswordComplex];
            if (isDefaultPasswordComplex == null)
                throw new UserFriendlyException("Not found Password Complex Config Seed Data");

            string password = PasswordUtils.GeneratePassword(12, bool.Parse(isDefaultPasswordComplex));

            var adminUserForHost = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            if (adminUserForHost == null)
            {
                var user = new User
                {
                    TenantId = null,
                    UserName = AbpUserBase.AdminUserName,
                    Name = "admin",
                    Surname = "admin",
                    EmailAddress = emailHost,
                    IsEmailConfirmed = true,
                    IsActive = true
                };

                user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(user, password);
                user.SetNormalizedNames();

                adminUserForHost = _context.Users.Add(user).Entity;
                _context.SaveChanges();

                // Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(null, adminUserForHost.Id, adminRoleForHost.Id));
                _context.SaveChanges();

                _context.SaveChanges();
            }
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
                    if (grantedPermissionsByRole != null)
                    {
                        var permissions = PermissionFinder.GetAllPermissions(new TalentV2AuthorizationProvider())
                                            .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host) && 
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
