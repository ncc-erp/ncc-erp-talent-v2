using Newtonsoft.Json;

namespace TalentV2.WebServices.ExternalServices.MezonWebhooks.Dtos
{
    public class MezonMention
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("s")]
        public int Start { get; set; }

        [JsonProperty("e")]
        public int End { get; set; }
    }
}
