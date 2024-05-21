using Newtonsoft.Json;

namespace TalentV2.DomainServices.CVAutomation.Dto
{
    public class CVExtractionData
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("dob")]
        public string Birthday { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }
}