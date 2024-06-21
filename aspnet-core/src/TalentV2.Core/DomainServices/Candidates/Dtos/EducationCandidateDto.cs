using Abp.AutoMapper;
using TalentV2.Entities;
using TalentV2.ModelExtends;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class EducationCandidateDto : CreateUpdateAudit
    {
        public long Id { get; set; }
        public long CVId { get; set; }
        public long EducationId { get; set; }
        public string EducationName { get; set; }
        public long EducationTypeId { get; set; }
        public string EducationTypeName { get; set; }
    }
    [AutoMapTo(typeof(CVEducation))]
    public class CreateUpdateEducationCandidateDto
    {
        public long Id { get; set; }
        public long CVId { get; set; }
        public long EducationId { get; set; }
    }
    public class CVEducationDto
    {
        public long Id { get; set; }
        public string EducationName { get; set; }
    }
}
