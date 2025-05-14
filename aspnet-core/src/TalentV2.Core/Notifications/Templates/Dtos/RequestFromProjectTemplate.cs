using TalentV2.Constants.Enum;

namespace TalentV2.Notifications.Templates.Dtos
{
    public class RequestFromProjectTemplate
    {
        public long RequestId { get; set; }
        public UserType UserType { get; set; }
        public string SubPositionName { get; set; }
        public string BranchName { get; set; }
        public string Note { get; set; }
        public string URL { get; set; }
        public Level Level { get; set; }
    }
}
