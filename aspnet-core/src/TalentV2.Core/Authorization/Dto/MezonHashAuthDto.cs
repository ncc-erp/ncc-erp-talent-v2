using Newtonsoft.Json;

namespace TalentV2.Authorization.Dto
{
    public class MezonHashAuthDto
    {
        [JsonProperty("hashKey")]
        public string HashKey { get; set; }
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("tenancyName")]
        public string TenancyName { get; set; } = "NCC";
    }
}
