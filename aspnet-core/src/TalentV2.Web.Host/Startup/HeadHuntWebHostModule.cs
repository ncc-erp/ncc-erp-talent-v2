using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using TalentV2.Configuration;
using Abp.Threading.BackgroundWorkers;
using TalentV2.BackgroundWorker;

namespace TalentV2.Web.Host.Startup
{
    [DependsOn(
       typeof(TalentV2WebCoreModule))]
    public class TalentV2WebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public TalentV2WebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(TalentV2WebHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            Logger.Info("PostInitialize() start");
            base.PostInitialize();

            var worker = IocManager.Resolve<IBackgroundWorkerManager>();
            worker.Add(IocManager.Resolve<NoticeInterviewResultWorker>());
            worker.Add(IocManager.Resolve<NoticeInterviewWorker>());
            Configuration.BackgroundJobs.IsJobExecutionEnabled = true;
            Logger.Info("PostInitialize() done");
        }
    }
}
