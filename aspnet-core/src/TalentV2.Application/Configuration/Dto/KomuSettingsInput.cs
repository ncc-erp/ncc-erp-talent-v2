using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Configuration.Dto
{
    public class KomuSettingsInput
    {
        public string KomuSetting { get; set; }
        public bool IsSendNotify { get; set; }
        public string SecretCode { get; set; }
        public string ChannelIdDevMode { get; set; }
    }

    public class KomuSettingsInputDto
    {
        public bool IsSendNotify { get; set; }
    }
}
