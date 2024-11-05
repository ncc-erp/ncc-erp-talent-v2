namespace TalentV2.DomainServices.CVAutomation.Dto
{
    public class CVScanResultFromFireBase
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Dob { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Position { get; set; }
        public string Note { get; set; }
        public byte[] CVData { get; set; }
    }
}
