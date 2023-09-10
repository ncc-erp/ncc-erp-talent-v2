using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    [AutoMapTo(typeof(RequestCVCapabilityResult))]
    public class UpdateCapabilityCandidateDto
    {
        public long Id { get; set; }
        public long RequestCVId { get; set; }
        public long CapabilityId { get; set; }
        public int Score { get; set; }
        public string Note { get; set; }
        public int Factor { get; set; }
    }
}
