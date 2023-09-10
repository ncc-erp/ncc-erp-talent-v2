using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class ValidCandidateDto
    {
        public long CVId { get; set; }
        public UserType UserType { get; set; }
    }
}
