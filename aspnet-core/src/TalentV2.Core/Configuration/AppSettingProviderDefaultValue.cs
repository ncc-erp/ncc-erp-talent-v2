using System.Security.Cryptography.X509Certificates;

namespace TalentV2.Configuration
{
    public class AppSettingProviderDefaultValue
    {
        public string UiTheme { get; set; }
        public string GoogleClientAppId { get; set; }
        public string StorageLocation { get; set; }
        public string KomuHRITChannelId { get; set; }
        public string KomuResourceRequestStaffChannelId { get; set; }
        public string KomuResourceRequestInternChannelId { get; set; }
        public string HRMURLSetting { get; set; }
        public string HRMSecurityCodeSetting { get; set; }
        public string ProjectURLSetting { get; set; }
        public string ProjectSecurityCodeSetting { get; set; }
        public string TimesheetURLSetting { get; set; }
        public string TimesheetSecurityCodeSetting { get; set; }
        public string TimesheetAutoUpdateSetting { get; set; }
        public string TalentSecurityCode { get; set; }
        public string NoticeInterviewStartAtHour { get; set; }
        public string NoticeInterviewEndAtHour { get; set; }
        public string NoticeInterviewMinutes { get; set; }
        public string NoticeInterviewResultMinutes { get; set; }
        public string IsNoticeInterviewViaChannel { get; set; }
        public string NoticeInterviewScheduleChannel { get; set; }
        public string NoticeInterviewResultChannel { get; set; }
        public string GoogleClientAppEnable { get; set; }
        public string EnableNormalLogin { get; set; }
    }
}
