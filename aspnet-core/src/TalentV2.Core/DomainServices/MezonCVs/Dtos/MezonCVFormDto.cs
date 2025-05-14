using System;
using System.Collections.Generic;
using TalentV2.DomainServices.Categories.Dtos;

namespace TalentV2.DomainServices.MezonCVs.Dtos
{
    public class MezonCVFormDto
    {
        public List<BranchDto> Branches { get; set; }
        public List<SubPositionDto> SubPositions { get; set; }
        public List<CVSourceDto> CVSources { get; set; }
    }
}
