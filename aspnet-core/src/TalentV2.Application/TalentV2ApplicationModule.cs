using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Abp.Timing;
using TalentV2.Authorization;
using TalentV2.BackgroundWorker;
using TalentV2.Timing;

namespace TalentV2
{
    [DependsOn(
        typeof(TalentV2CoreModule), 
        typeof(AbpAutoMapperModule))]
    public class TalentV2ApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Logger.Info("PreInitialize() start");
            Configuration.Authorization.Providers.Add<TalentV2AuthorizationProvider>();
            //Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.MultiTenancy.IsEnabled = TalentV2Consts.MultiTenancyEnabled;
            Configuration.MultiTenancy.TenantIdResolveKey = "Abp-TenantId";

            Logger.Info("PreInitialize() done");
        }

        public override void Initialize()
        {
            Logger.Info("Initialize() start");
            var thisAssembly = typeof(TalentV2ApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );

            Logger.Info("Initialize() done");
        }

    }
}
