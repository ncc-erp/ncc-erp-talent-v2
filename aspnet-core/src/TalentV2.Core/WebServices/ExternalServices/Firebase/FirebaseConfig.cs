namespace TalentV2.WebServices.ExternalServices.Firebase
{
    public class FirebaseConfig
    {
        public int IntervalMilisecond { get; set; }
        public bool RunFirebaseBackgroundService { get; set; }
        public string SecretKey { get; set; }
        public string Url { get; set; }
        public int FileSize { get; set; }
    }
}
