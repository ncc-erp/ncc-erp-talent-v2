namespace TalentV2.DomainServices.Reports.Dtos
{
    public class CandidateQuantityByEducationReportDto : GeneralEducationInformationDto
    {
        public int TotalCV { get; set; }
        public int PassCV { get; set; }
        public int PassTest { get; set; }
        public int PassInterview { get; set; }
        public int Onboard { get; set; }
        public int Other { get; set; }
    }
}
