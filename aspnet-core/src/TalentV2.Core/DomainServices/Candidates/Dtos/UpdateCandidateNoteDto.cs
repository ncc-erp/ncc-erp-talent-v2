using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class UpdateCandidateNoteDto
    {
        public long CVId { get; set; }
        public string Note { get; set; }
    }
}
