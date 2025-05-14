using Newtonsoft.Json;

namespace TalentV2.WebServices.ExternalServices.MezonWebhooks.Dtos
{
    public class MezonWebhookMessageDto
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "hook";

        [JsonProperty("message")]
        public MezonMessage MezonMessage { get; set; }
    }
}
