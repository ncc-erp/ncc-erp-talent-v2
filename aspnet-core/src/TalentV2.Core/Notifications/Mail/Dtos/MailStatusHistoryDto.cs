using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.Notifications.Mail.Dtos
{
    public class MailStatusHistoryDto
    {
        public long Id { get; set; }
        public long CVId { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        public string Subject { get; set; }
        public MailFuncEnum MailFuncType { get; set; }
    }
}
