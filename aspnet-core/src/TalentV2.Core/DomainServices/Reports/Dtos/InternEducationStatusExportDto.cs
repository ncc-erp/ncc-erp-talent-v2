using System;
using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.Reports.Dtos
{
    internal class InternEducationStatusExportDto
    {
        public long BranchId { get; set; }
        public long EducationId { get; set; }
        public string EducationName { get; set; }
        public CVStatus CVStatus { get; set; }
        public RequestCVStatus CandidateStatus { get; set; }
        public DateTime CVStatusTime { get; set; }
    }
}