using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using HeadHunt.EntityFrameworkCore;
using HeadHunt.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace HeadHunt.Web.Tests
{
    [DependsOn(
        typeof(HeadHuntWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class HeadHuntWebTestModule : AbpModule
    {
        public HeadHuntWebTestModule(HeadHuntEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(HeadHuntWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(HeadHuntWebMvcModule).Assembly);
        }
    }
}