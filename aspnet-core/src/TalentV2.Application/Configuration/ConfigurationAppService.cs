using Abp.Authorization;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.Configuration.Dto;
using TalentV2.DomainServices.Dto;
using TalentV2.WebServices.InternalServices.HRM;
using TalentV2.WebServices.InternalServices.LMS;

namespace TalentV2.Configuration
{
    public class ConfigurationAppService : TalentV2AppServiceBase, IConfigurationAppService
    {

        private static IConfiguration _appConfiguration;
        private readonly LMSService _lMSService;
        private readonly HRMService _hrmService;
        public ConfigurationAppService(IConfiguration appConfiguration, LMSService lMSService, HRMService hrmService)
        {
            _appConfiguration = appConfiguration;
            _lMSService = lMSService;
            _hrmService = hrmService;
        }

        [AbpAuthorize]
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
        [AbpAllowAnonymous]
        public async Task<string> GetGoogleClientAppId()
        {
            return await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.GoogleClientAppId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Configurations_EditEmailSettings)]
        public async Task<EmailConfigurationInput> SetEmailSetting(EmailConfigurationInput input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromAddress, input.DefaultAddress);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromDisplayName, input.DisplayName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Host, input.Host);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Port, input.Port);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UserName, input.UserName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Password, input.Password);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.EnableSsl, input.EnableSsl);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials, input.UseDefaultCredentials);
            return new EmailConfigurationInput
            {
                DefaultAddress = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromAddress),
                DisplayName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromDisplayName),
                Host = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Host),
                Port = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Port),
                UserName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UserName),
                Password = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Password),
                EnableSsl = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.EnableSsl),
                UseDefaultCredentials = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials)
            };
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Configurations_ViewEmailSettings)]
        public async Task<EmailConfigurationInput> GetEmailSetting()
        {
            return new EmailConfigurationInput
            {
                DefaultAddress = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromAddress),
                DisplayName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromDisplayName),
                Host = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Host),
                Port = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Port),
                UserName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UserName),
                Password = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Password),
                EnableSsl = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.EnableSsl),
                UseDefaultCredentials = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials)
            };
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Configurations_ViewKomuSettings)]
        public async Task<KomuSettingsInput> GetKomuSettings()
        {
            return new KomuSettingsInput
            {
                KomuSetting = _appConfiguration.GetValue<string>("KomuService:BaseAddress"),
                IsSendNotify = _appConfiguration.GetValue<string>("KomuService:EnableKomuNotification") == "true" ? true : false,
                SecretCode = _appConfiguration.GetValue<string>("KomuService:SecurityCode"),
                ChannelIdDevMode = _appConfiguration.GetValue<string>("KomuService:ChannelIdDevMode")
            };
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Configurations_ViewChannelHRITSettings)]
        public async Task<DiscordChannelInput> GetDiscordChannelHRIT()
        {
            return new DiscordChannelInput
            {
                ChannelHRITId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.KomuHRITChannelId),
                KomuResourceRequestInternChannelId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.KomuResourceRequestInternChannelId),
                KomuResourceRequestStaffChannelId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.KomuResourceRequestStaffChannelId)
            };
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Configurations_EditChannelHRITSettings)]
        public async Task<DiscordChannelInput> SetDiscordChannelHRIT(DiscordChannelInput input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.KomuHRITChannelId, input.ChannelHRITId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.KomuResourceRequestInternChannelId, input.KomuResourceRequestInternChannelId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.KomuResourceRequestStaffChannelId, input.KomuResourceRequestStaffChannelId);
            return new DiscordChannelInput
            {
                ChannelHRITId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.KomuHRITChannelId),
                KomuResourceRequestInternChannelId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.KomuResourceRequestInternChannelId),
                KomuResourceRequestStaffChannelId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.KomuResourceRequestStaffChannelId)
            };
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Configurations_ViewHRMSettings)]
        public async Task<InternalToolSettingInput> GetHRMSettings()
        {
            return new InternalToolSettingInput
            {
                URL = _appConfiguration.GetValue<string>("HRMService:BaseAddress"),
                SecurityCode = _appConfiguration.GetValue<string>("HRMService:SecurityCode")
            };
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Configurations_ViewAutoBotSettings)]
        public async Task<InternalToolSettingInput> GetAutoBotSettings()
        {
            return new InternalToolSettingInput
            {
                URL = _appConfiguration.GetValue<string>("AutobotService:BaseAddress"),
                SecurityCode = _appConfiguration.GetValue<string>("AutobotService:SecurityCode")
            };
        }

        [HttpPost]
        //[AbpAuthorize(PermissionNames.Pages_Configurations_EditTimesheetSettings)]
        public async Task<TimesheetSettingInput> SetTimesheetSettings(TimesheetSettingInput input)
        {
            string textIsAuto = "false";
            if (input.IsAutoUpdate)
                textIsAuto = "true";

            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.TimesheetAutoUpdateSetting, textIsAuto);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.TimesheetURLSetting, input.URL);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.TimesheetSecurityCodeSetting, input.SecurityCode);
            return input;
        }

        [HttpGet]
        //[AbpAuthorize(PermissionNames.Pages_Configurations_ViewProjectSettings)]
        public async Task<InternalToolSettingInput> GetProjectSettings()
        {
            return new InternalToolSettingInput
            {
                URL = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.ProjectURLSetting),
                SecurityCode = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.ProjectSecurityCodeSetting)
            };
        }

        [HttpPost]
        //[AbpAuthorize(PermissionNames.Pages_Configurations_EditProjectSettings)]
        public async Task<InternalToolSettingInput> SetProjectSettings(InternalToolSettingInput input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.ProjectURLSetting, input.URL);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.ProjectSecurityCodeSetting, input.SecurityCode);
            return input;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Configurations_ViewLMSSettings)]
        public async Task<LMSSettingsDto> GetLMSSettings()
        {
            return new LMSSettingsDto
            {
                URL = _appConfiguration.GetValue<string>("LMSService:BaseAddress"),
                FEAddress = _appConfiguration.GetValue<string>("LMSService:FEAddress"),
                SecurityCode = _appConfiguration.GetValue<string>("LMSService:SecurityCode")
            };
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Configurations_ViewGoogleClientAppSettings)]
        public async Task<GoogleClientAppSettingDto> GetGoogleClientAppSettings()
        {
            string textEnableNormalLogin = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.EnableNormalLogin);
            return new GoogleClientAppSettingDto
            {
                GoogleClientAppId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.GoogleClientAppId),
                EnableNormalLogin = textEnableNormalLogin == "true" ? true : false
            };
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Configurations_EditGoogleClientAppSettings)]
        public async Task<GoogleClientAppSettingDto> SetGoogleClientAppSettings(GoogleClientAppSettingDto input)
        {
            string txtEnableNormalLogin = "false";
            if (input.EnableNormalLogin)
                txtEnableNormalLogin = "true";
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.GoogleClientAppId, input.GoogleClientAppId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.EnableNormalLogin, txtEnableNormalLogin);
            return input;
        }

        [AbpAllowAnonymous]
        public async Task<bool> GetEnableNormalLogin()
        {
            return await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.EnableNormalLogin) == "true" ? true : false;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Configurations_EditTalentSecretCode)]
        public async Task<SecretCodeTalentInput> SetTalentSecretCode(SecretCodeTalentInput input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.TalentSecurityCode, input.SecretCode);
            return input;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Configurations_ViewTalentSecretCode)]
        public async Task<SecretCodeTalentInput> GetTalentSecretCode()
        {
            return new SecretCodeTalentInput
            {
                SecretCode = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.TalentSecurityCode)
            };
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Configurations_ViewTalentNotifyInterviewSettings)]
        public async Task<NoticeInterviewSettingDto> GetNoticeInterviewSetting()
        {
            return new NoticeInterviewSettingDto
            {
                NoticeInterviewStartAtHour = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NoticeInterviewStartAtHour),
                NoticeInterviewEndAtHour = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NoticeInterviewEndAtHour),
                NoticeInterviewMinutes = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NoticeInterviewMinutes),
                NoticeInterviewResultMinutes = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NoticeInterviewResultMinutes),
                IsToChannel = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.IsNoticeInterviewViaChannel),
                ScheduleChannel = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NoticeInterviewScheduleChannel),
                ResultChannel = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NoticeInterviewResultChannel),
                TalentGeneralChannel = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NoticeTalentGeneralChannel),
            };
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Configurations_EditTalentNotifyInterviewSettings)]
        public async Task<NoticeInterviewSettingDto> SetNoticeInterviewSetting(NoticeInterviewSettingDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.NoticeInterviewStartAtHour, input.NoticeInterviewStartAtHour);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.NoticeInterviewEndAtHour, input.NoticeInterviewEndAtHour);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.NoticeInterviewMinutes, input.NoticeInterviewMinutes);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.NoticeInterviewResultMinutes, input.NoticeInterviewResultMinutes);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.IsNoticeInterviewViaChannel, input.IsToChannel);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.NoticeInterviewScheduleChannel, input.ScheduleChannel);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.NoticeInterviewResultChannel, input.ResultChannel);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.NoticeTalentGeneralChannel, input.TalentGeneralChannel);
            return input;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Configurations_ViewTalentNotifyCVAutomationSettings)]
        public async Task<NoticeCVAutomationSettingDto> GetNoticeCVAutomationSettings()
        {
            return new NoticeCVAutomationSettingDto
            {
                Enabled = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.CVAutomationEnabled),
                RepeatTimeInMinutes = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.CVAutomationRepeatTimeInMinutes),
                NoticeStartAtHour = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.CVAutomationNoticeStartAtHour),
                NoticeEndAtHour = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.CVAutomationNoticeEndAtHour),
                NoticeMode = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.CVAutomationNoticeMode),
                NoticeChannelId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.CVAutomationNoticeChannelId),
                NotifyToUser = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.CVAutomationNotifyToUser),
            };
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Configurations_EditTalentNotifyCVAutomationSettings)]
        public async Task<NoticeCVAutomationSettingDto> SetNoticeCVAutomationSettings(NoticeCVAutomationSettingDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.CVAutomationEnabled, input.Enabled);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.CVAutomationRepeatTimeInMinutes, input.RepeatTimeInMinutes);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.CVAutomationNoticeStartAtHour, input.NoticeStartAtHour);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.CVAutomationNoticeEndAtHour, input.NoticeEndAtHour);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.CVAutomationNoticeMode, input.NoticeMode);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.CVAutomationNoticeChannelId, input.NoticeChannelId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.CVAutomationNotifyToUser, input.NotifyToUser);
            return input;
        }

        [HttpGet]
        public GetResultConnectDto CheckConnectToHRM()
        {
            return _hrmService.CheckConnectToHRM();
        }

        [HttpGet]
        public GetResultConnectDto CheckConnectToLMS()
        {
            return _lMSService.CheckConnectToLMS();
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.PermissionNames_Pages_Configurations_ConfigureContestUrl)]
        public async Task<ContestUrlDto> GetContestUrl()
        {
            return new ContestUrlDto
            {
                ContestUrl = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.TalentContestUrl)
            };
        }
    }
}
