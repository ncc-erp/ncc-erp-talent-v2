namespace TalentV2.WebServices.ExternalServices.Mezon.Dtos
{
    public class OAuth2Request
    {
        public string Code { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
    }
}
