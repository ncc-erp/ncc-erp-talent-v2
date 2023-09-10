using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Configuration.Dto
{
    public class InternalToolSettingInput
    {
        public string URL { get; set; }
        public string SecurityCode { get; set; }
    }
    public class TimesheetSettingInput : InternalToolSettingInput
    {
        public bool IsAutoUpdate { get; set; }
    }
}
