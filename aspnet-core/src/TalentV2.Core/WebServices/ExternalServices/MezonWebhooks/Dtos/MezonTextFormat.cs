using Newtonsoft.Json;

namespace TalentV2.WebServices.ExternalServices.MezonWebhooks.Dtos
{
    public class MezonTextFormat
    {
        [JsonProperty("type")]
        public string MezonTextFormatType { get; set; }

        [JsonProperty("s")]
        public int Start { get; set; }

        [JsonProperty("e")]
        public int End { get; set; }
    }
}
