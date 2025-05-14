using Newtonsoft.Json;
using System.Collections.Generic;

namespace TalentV2.WebServices.ExternalServices.MezonWebhooks.Dtos
{
    public class MezonMessage
    {
        [JsonProperty("t")]
        public string Content { get; set; }

        [JsonProperty("mk")]
        public List<MezonTextFormat> MezonTextFormats { get; set; }

        [JsonProperty("mentions")]
        public List<MezonMention> MezonMentions { get; set; }

        [JsonProperty("images")]
        public List<MezonImage>? MezonImages { get; set; }

    }
}
