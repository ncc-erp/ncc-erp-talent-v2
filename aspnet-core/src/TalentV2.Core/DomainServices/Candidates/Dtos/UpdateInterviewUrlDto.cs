using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class UpdateInterviewUrlDto
    {
        public long RequestCVId { get; set; }
        public string Url { get; set; }
    }
}
