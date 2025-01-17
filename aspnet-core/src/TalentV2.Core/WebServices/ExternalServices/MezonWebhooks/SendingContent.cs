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
        public List<Markdown> mk { get; set; }
        //public List<Link> lk { get; set; }
    }

    public class Markdown
    {
        public string type { get; set; }
        public int s { get; set; }
        public int e { get; set; }
    }

    public class Link
    {
        public int s { get; set; }
        public int e { get; set; }
    }
}
