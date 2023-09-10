using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Configuration.Dto
{
    public class NoticeInterviewSettingDto
    {
        public string NoticeInterviewStartAtHour { get; set; }
        public string NoticeInterviewEndAtHour { get; set; }
        public string NoticeInterviewMinutes { get; set; }
        public string NoticeInterviewResultMinutes { get; set; }
        public string IsToChannel { get; set; }
        public string ScheduleChannel { get; set; }
        public string ResultChannel { get; set; }
    }
}
