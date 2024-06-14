using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Security;
using Abp.Zero;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NccCore.Extension;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;
using TalentV2.Configuration;
using TalentV2.Debugging;
using TalentV2.Localization;
using TalentV2.MultiTenancy;

namespace TalentV2
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class TalentV2CoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Logger.Info("PreInitialize() start");
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);
            //Configuration.BackgroundJobs.IsJobExecutionEnabled = true;
            TalentV2LocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = TalentV2Consts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            ConfigureSettingProvider();

            Configuration.Localization.Languages.Add(new LanguageInfo("fa", "فارسی", "famfamfam-flags ir"));

            ConfigureDefaultPassPhrase();

            Logger.Info("PreInitialize() done");
        }

        private void ConfigureDefaultPassPhrase()
        {
            if (IocManager.IsRegistered<IWebHostEnvironment>())
            {
                var config = IocManager.Resolve<IWebHostEnvironment>().GetConfigurationRoot();
                var defaultPassPhrase = config.GetValue<string>("DefaultPassPhrase");
                if (!defaultPassPhrase.IsEmpty() && !DebugHelper.IsDebug)
                {
                    Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = defaultPassPhrase;
                    SimpleStringCipher.DefaultPassPhrase = defaultPassPhrase;
                }
            }
        }

        private void ConfigureSettingProvider()
        {
            IocManager.Register<AppSettingProviderDefaultValue>();
            if (IocManager.IsRegistered<IWebHostEnvironment>())
            {
                var config = IocManager.Resolve<IWebHostEnvironment>().GetConfigurationRoot();
                var appSettingProviderDefaultValue = IocManager.Resolve<AppSettingProviderDefaultValue>();
                var appSettingValueProvider = config.GetSection("AppSettingProviders");

                appSettingProviderDefaultValue.EnableNormalLogin = appSettingValueProvider.GetValue<string>(AppSettingNames.EnableNormalLogin);
                appSettingProviderDefaultValue.GoogleClientAppEnable = appSettingValueProvider.GetValue<string>(AppSettingNames.GoogleClientAppEnable);
                appSettingProviderDefaultValue.GoogleClientAppId = appSettingValueProvider.GetValue<string>(AppSettingNames.GoogleClientAppId);
                appSettingProviderDefaultValue.HRMSecurityCodeSetting = appSettingValueProvider.GetValue<string>(AppSettingNames.HRMSecurityCodeSetting);
                appSettingProviderDefaultValue.HRMURLSetting = appSettingValueProvider.GetValue<string>(AppSettingNames.HRMURLSetting);
                appSettingProviderDefaultValue.IsNoticeInterviewViaChannel = appSettingValueProvider.GetValue<string>(AppSettingNames.IsNoticeInterviewViaChannel);
                appSettingProviderDefaultValue.KomuHRITChannelId = appSettingValueProvider.GetValue<string>(AppSettingNames.KomuHRITChannelId);
                appSettingProviderDefaultValue.KomuResourceRequestInternChannelId = appSettingValueProvider.GetValue<string>(AppSettingNames.KomuResourceRequestInternChannelId);
                appSettingProviderDefaultValue.KomuResourceRequestStaffChannelId = appSettingValueProvider.GetValue<string>(AppSettingNames.KomuResourceRequestStaffChannelId);
                appSettingProviderDefaultValue.NoticeInterviewEndAtHour = appSettingValueProvider.GetValue<string>(AppSettingNames.NoticeInterviewEndAtHour);
                appSettingProviderDefaultValue.NoticeInterviewMinutes = appSettingValueProvider.GetValue<string>(AppSettingNames.NoticeInterviewMinutes);
                appSettingProviderDefaultValue.NoticeInterviewResultChannel = appSettingValueProvider.GetValue<string>(AppSettingNames.NoticeInterviewResultChannel);
                appSettingProviderDefaultValue.NoticeInterviewResultMinutes = appSettingValueProvider.GetValue<string>(AppSettingNames.NoticeInterviewResultMinutes);
                appSettingProviderDefaultValue.NoticeInterviewScheduleChannel = appSettingValueProvider.GetValue<string>(AppSettingNames.NoticeInterviewScheduleChannel);
                appSettingProviderDefaultValue.NoticeInterviewStartAtHour = appSettingValueProvider.GetValue<string>(AppSettingNames.NoticeInterviewStartAtHour);
                appSettingProviderDefaultValue.ProjectSecurityCodeSetting = appSettingValueProvider.GetValue<string>(AppSettingNames.ProjectSecurityCodeSetting);
                appSettingProviderDefaultValue.ProjectURLSetting = appSettingValueProvider.GetValue<string>(AppSettingNames.ProjectURLSetting);
                appSettingProviderDefaultValue.StorageLocation = appSettingValueProvider.GetValue<string>(AppSettingNames.StorageLocation);
                appSettingProviderDefaultValue.TalentSecurityCode = appSettingValueProvider.GetValue<string>(AppSettingNames.TalentSecurityCode);
                appSettingProviderDefaultValue.TimesheetAutoUpdateSetting = appSettingValueProvider.GetValue<string>(AppSettingNames.TimesheetAutoUpdateSetting);
                appSettingProviderDefaultValue.TimesheetSecurityCodeSetting = appSettingValueProvider.GetValue<string>(AppSettingNames.TimesheetSecurityCodeSetting);
                appSettingProviderDefaultValue.TimesheetURLSetting = appSettingValueProvider.GetValue<string>(AppSettingNames.TimesheetURLSetting);
                appSettingProviderDefaultValue.UiTheme = appSettingValueProvider.GetValue<string>(AppSettingNames.UiTheme);
                appSettingProviderDefaultValue.TalentContestUrl = appSettingValueProvider.GetValue<string>(AppSettingNames.TalentContestUrl);
                appSettingProviderDefaultValue.CVAutomationEnabled = appSettingValueProvider.GetValue<string>(AppSettingNames.CVAutomationEnabled);
                appSettingProviderDefaultValue.CVAutomationRepeatTimeInMinutes = appSettingValueProvider.GetValue<string>(AppSettingNames.CVAutomationRepeatTimeInMinutes);
                appSettingProviderDefaultValue.CVAutomationNoticeStartAtHour = appSettingValueProvider.GetValue<string>(AppSettingNames.CVAutomationNoticeStartAtHour);
                appSettingProviderDefaultValue.CVAutomationNoticeEndAtHour = appSettingValueProvider.GetValue<string>(AppSettingNames.CVAutomationNoticeEndAtHour);
                appSettingProviderDefaultValue.CVAutomationNoticeMode = appSettingValueProvider.GetValue<string>(AppSettingNames.CVAutomationNoticeMode);
                appSettingProviderDefaultValue.CVAutomationNoticeChannelId = appSettingValueProvider.GetValue<string>(AppSettingNames.CVAutomationNoticeChannelId);
                appSettingProviderDefaultValue.CVAutomationNotifyToUser = appSettingValueProvider.GetValue<string>(AppSettingNames.CVAutomationNotifyToUser);
            }
            Configuration.Settings.Providers.Add<AppSettingProvider>();
        }

        public override void Initialize()
        {
            Logger.Info("Initialize() start");
            IocManager.RegisterAssemblyByConvention(typeof(TalentV2CoreModule).GetAssembly());
            Logger.Info("Initialize() done");
        }

    }
}
