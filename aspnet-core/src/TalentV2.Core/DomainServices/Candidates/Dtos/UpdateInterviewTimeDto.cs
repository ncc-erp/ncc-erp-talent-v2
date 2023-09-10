using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class UpdateInterviewTimeDto
    {
        public long RequestCVId { get; set; }
        public DateTime InterviewTime { get; set; }
    }
}
