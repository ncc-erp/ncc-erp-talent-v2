using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Notifications.Komu.Dtos
{
    public class KomuMessageDto
    {
        [JsonProperty("pathImage")]
        public string PathImage { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
