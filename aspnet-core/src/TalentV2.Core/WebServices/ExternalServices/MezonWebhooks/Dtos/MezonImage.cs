using Newtonsoft.Json;

namespace TalentV2.WebServices.ExternalServices.MezonWebhooks.Dtos
{
    public class MezonImage
    {
        [JsonProperty("fn")]
        public string FileName { get; set; }

        [JsonProperty("sz")]
        public long Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("ft")]
        public string FileType { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }
    }
}
