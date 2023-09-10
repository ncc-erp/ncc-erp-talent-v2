using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.Notifications.Mail.Dtos
{
    public class MailDto
    {
        public long Id { get; set; }
        public MailFuncEnum Type { get; set; }
        public string Name { get; set; }
        public string CCs { get; set; }
        public string[] ArrCCs { get => string.IsNullOrEmpty(CCs) ? new string[0] : CCs.Split(",").ToArray(); }
        public string Description { get; set; }
    }
    public class MailPreviewInfoDto
    {
        public long TemplateId { get; set; }
        public MailFuncEnum MailFuncType { get; set; }
        public string Subject { get; set; }
        public string BodyMessage { get; set; }
        public string To { get; set; }
        public string[] PropertiesSupport { get; set; }
        public List<string> CCs { get; set; } = new List<string>();
    }
    public class ResultTemplateEmail<T> where T : class
    {
        public T Result { get; set; }
        public string[] PropertiesSupport { get => typeof(T).GetProperties().Select(s => s.Name).ToArray(); }
    }
}
