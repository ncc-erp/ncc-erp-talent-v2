using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.Notifications.Komu.Dtos
{
    public class NotificationRequestFromProject
    {
        public long RequestId { get; set; }
        public UserType UserType { get; set; }
        public Level Level { get; set; }
        public long SubPositionId { get; set; }
        public long BranchId { get; set; }
        public string Note { get; set; }
        public string URI { get; set; }
    }

    public class MessageNotificationRequestFromProject
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
