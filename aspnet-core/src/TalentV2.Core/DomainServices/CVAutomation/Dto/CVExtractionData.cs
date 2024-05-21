using System.Text.Json.Serialization;

namespace TalentV2.DomainServices.CVAutomation.Dto
{
    public class CVExtractionData
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("dob")]
        public string Birthday { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("position")]
        public string Position { get; set; }
    }
}