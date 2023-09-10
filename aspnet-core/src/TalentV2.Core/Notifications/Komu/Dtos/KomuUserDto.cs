using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Notifications.Komu.Dtos
{
    public class KomuUserDto
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("userid")]
        public ulong? KomuUserId { get; set; }
    }
}
