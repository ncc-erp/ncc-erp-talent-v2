using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.Notifications.Templates.Dtos
{
    public class MailInfo
    {
        public string Name { get; set; }
        public string BodyMessage { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
    }
}
