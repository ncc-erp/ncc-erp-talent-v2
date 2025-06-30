namespace TalentV2.DomainServices.Reports.Dtos
{
    public class CandidateDensityByEducationReportDto : GeneralEducationInformationDto
    {
        public double PassCV { get; set; }
        public double PassTest { get; set; }
        public double PassInterview { get; set; }
        public double Onboard { get; set; }
        public double Other { get; set; }
    }
}
