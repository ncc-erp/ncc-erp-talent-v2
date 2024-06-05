namespace TalentV2.Configuration.Dto
{
    public class NoticeCVAutomationSettingDto
    {
        public string Enabled { get; set; }
        public string RepeatTimeInMinutes { get; set; }
        public string NoticeStartAtHour { get; set; }
        public string NoticeEndAtHour { get; set; }
        public string NoticeMode { get; set; }
        public string NoticeChannelId { get; set; }
        public string NotifyToUser { get; set; }
    }
}
