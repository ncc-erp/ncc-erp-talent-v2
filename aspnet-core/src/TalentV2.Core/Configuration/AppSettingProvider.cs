using Abp.Configuration;
using System.Collections.Generic;

namespace TalentV2.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        private readonly AppSettingProviderDefaultValue _defaultValue;
        public AppSettingProvider(AppSettingProviderDefaultValue defaultValue)
        {
            _defaultValue = defaultValue;
        }

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition
                (
                    AppSettingNames.UiTheme,
                    _defaultValue.UiTheme,
                    scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User,
                    clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()
                ),
                new SettingDefinition
                (
                    AppSettingNames.GoogleClientAppId,
                    _defaultValue.GoogleClientAppId,
                    scopes:SettingScopes.Application| SettingScopes.Tenant
                ),

                #region KomuDiscord
                new SettingDefinition
                (
                    AppSettingNames.KomuHRITChannelId,
                    _defaultValue.KomuHRITChannelId,
                    scopes:SettingScopes.Application| SettingScopes.Tenant
                ),
                new SettingDefinition
                (
                    AppSettingNames.KomuResourceRequestInternChannelId,
                    _defaultValue.KomuResourceRequestInternChannelId,
                    scopes:SettingScopes.Application| SettingScopes.Tenant
                ),
                new SettingDefinition
                (
                    AppSettingNames.KomuResourceRequestStaffChannelId,
                    _defaultValue.KomuResourceRequestStaffChannelId,
                    scopes:SettingScopes.Application| SettingScopes.Tenant
                ),
                new SettingDefinition
                (
                    AppSettingNames.TalentSecurityCode,
                    _defaultValue.TalentSecurityCode,
                    scopes:SettingScopes.Application| SettingScopes.Tenant
                ),
                new SettingDefinition
                (
                    AppSettingNames.TalentContestUrl,
                    _defaultValue.TalentContestUrl,
                    scopes:SettingScopes.Application| SettingScopes.Tenant
                ),
                #endregion 

                #region NotifyInterViewing
                new SettingDefinition(
                    AppSettingNames.NoticeInterviewStartAtHour,
                    _defaultValue.NoticeInterviewStartAtHour,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.NoticeInterviewEndAtHour,
                    _defaultValue.NoticeInterviewEndAtHour,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.NoticeInterviewMinutes,
                    _defaultValue.NoticeInterviewMinutes,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.NoticeInterviewResultMinutes,
                    _defaultValue.NoticeInterviewResultMinutes,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.IsNoticeInterviewViaChannel,
                    _defaultValue.IsNoticeInterviewViaChannel,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.NoticeInterviewScheduleChannel,
                    _defaultValue.NoticeInterviewScheduleChannel,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.NoticeInterviewResultChannel,
                    _defaultValue.NoticeInterviewResultChannel,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                #endregion

                #region CVAutomationSettings
                new SettingDefinition(
                    AppSettingNames.CVAutomationEnabled,
                    _defaultValue.CVAutomationEnabled,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                    ),
                new SettingDefinition(
                    AppSettingNames.CVAutomationRepeatTimeInMinutes,
                    _defaultValue.CVAutomationRepeatTimeInMinutes,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                    ),
                new SettingDefinition(
                    AppSettingNames.CVAutomationNoticeStartAtHour,
                    _defaultValue.CVAutomationNoticeStartAtHour,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                    ),
                new SettingDefinition(
                    AppSettingNames.CVAutomationNoticeEndAtHour,
                    _defaultValue.CVAutomationNoticeEndAtHour,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                    ),
                new SettingDefinition(
                    AppSettingNames.CVAutomationNoticeMode,
                    _defaultValue.CVAutomationNoticeMode,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                    ),
                new SettingDefinition(
                    AppSettingNames.CVAutomationNoticeChannelId,
                    _defaultValue.CVAutomationNoticeChannelId,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                    ),
                new SettingDefinition(
                    AppSettingNames.CVAutomationNotifyToUser,
                    _defaultValue.CVAutomationNotifyToUser,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                    ),
                #endregion

                #region Internal Tools
                new SettingDefinition(
                    AppSettingNames.HRMURLSetting,
                    _defaultValue.HRMURLSetting,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.HRMSecurityCodeSetting,
                    _defaultValue.HRMSecurityCodeSetting,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.ProjectURLSetting,
                    _defaultValue.ProjectURLSetting,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.ProjectSecurityCodeSetting,
                    _defaultValue.ProjectSecurityCodeSetting,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.TimesheetURLSetting,
                    _defaultValue.TimesheetURLSetting,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.TimesheetAutoUpdateSetting,
                    _defaultValue.TimesheetAutoUpdateSetting,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.TimesheetSecurityCodeSetting,
                    _defaultValue.TimesheetSecurityCodeSetting,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                #endregion

                new SettingDefinition(
                    AppSettingNames.GoogleClientAppEnable,
                    _defaultValue.GoogleClientAppEnable,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
                new SettingDefinition(
                    AppSettingNames.EnableNormalLogin,
                    _defaultValue.EnableNormalLogin,
                    scopes:SettingScopes.Application | SettingScopes.Tenant
                ),
            };
        }
    }
}
