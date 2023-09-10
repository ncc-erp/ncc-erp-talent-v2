using System;
using System.Collections.Generic;
using System.Text;

namespace TalentV2.APIs.NccCVs.Project.Dto
{
    public class ChangeProjectAndTechnologyDto
    {
        public long ProjectId { get; set; }
        public List<long> TechnologyId { get; set; }
    }
}
