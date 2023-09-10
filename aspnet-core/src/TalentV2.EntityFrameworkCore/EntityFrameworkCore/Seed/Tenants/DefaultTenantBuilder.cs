using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.MultiTenancy;
using TalentV2.Editions;
using TalentV2.MultiTenancy;
using TalentV2.SeedData;
using Microsoft.Extensions.Options;
using Abp.Dependency;
using Abp.UI;
using Microsoft.Extensions.Configuration;
using TalentV2.EntityFrameworkCore.Seed.Emails;

namespace TalentV2.EntityFrameworkCore.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly TalentV2DbContext _context;

        public DefaultTenantBuilder(TalentV2DbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefaultTenant();
        }

        private void CreateDefaultTenant()
        {
            // Default tenant
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection(SeedDataConfig.SeedDataConfigKey);
            var defaultName = config[SeedDataConfig.DefaultTenantName];
            if(string.IsNullOrEmpty(defaultName))
                throw new UserFriendlyException("Not found Tenant Admin Name Config Seed Data");

            var defaultTenant = _context.Tenants.IgnoreQueryFilters().FirstOrDefault(t => t.TenancyName == defaultName);
            if (defaultTenant == null)
            {
                defaultTenant = new Tenant(defaultName, defaultName);

                var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                if (defaultEdition != null)
                {
                    defaultTenant.EditionId = defaultEdition.Id;
                }

                _context.Tenants.Add(defaultTenant);
                _context.SaveChanges();
            }
            new DefaultEmailSettingsCreator(_context, null).Create();
        }
    }
}
