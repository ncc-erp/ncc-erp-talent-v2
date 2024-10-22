using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.WebServices.ExternalServices.MezonWebhooks
{
    public class SendingContent
    {
        public string type { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string t { get; set; }
        public string username { get; set; }
    }
}
