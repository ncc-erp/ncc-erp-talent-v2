using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Controllers.Dtos
{
    public class TokenDto
    {
        public string googleToken { get; set; }
        public string secretCode { get; set; }
    }

    public class OAuth2TokenDto
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
