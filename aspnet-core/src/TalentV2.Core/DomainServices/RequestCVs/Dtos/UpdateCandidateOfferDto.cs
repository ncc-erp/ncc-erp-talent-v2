using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.RequestCVs.Dtos
{
    [AutoMapTo(typeof(RequestCV))]
    public class UpdateCandidateOfferDto
    {
        public long Id { get; set; }
        public Level? FinalLevel { get; set; }
        public RequestCVStatus Status { get; set; }
        public float Salary { get; set; }
        public DateTime? OnboardDate { get; set; }
        public string HRNote { get; set; }
    }
}
