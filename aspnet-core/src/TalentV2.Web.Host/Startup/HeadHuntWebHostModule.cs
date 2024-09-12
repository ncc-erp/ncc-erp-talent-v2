using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TalentV2.BackgroundWorker.Hangfire;
using TalentV2.Configuration;
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
			var workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
			foreach (var workerEntry in BackgroundWokerInitation.workers)
			{
				dynamic worker = workerEntry.Value;
				workerManager.Add(worker.Worker);
				RecurringJob.AddOrUpdate(workerEntry.Key, worker.Trigger, Cron.Never());
			};
			Configuration.BackgroundJobs.IsJobExecutionEnabled = true;
			Logger.Info("PostInitialize() done");
		}
	}
}
