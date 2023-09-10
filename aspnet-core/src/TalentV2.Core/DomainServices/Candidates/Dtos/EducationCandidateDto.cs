using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
