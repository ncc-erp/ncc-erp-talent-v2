using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    [AutoMapTo(typeof(RequestCV))]
    public class UpdateApplicationResultDto
    {
        public long RequestCvId { get; set; }
        public RequestCVStatus Status { get; set; }
        public Level? ApplyLevel { get; set; }
        public Level? FinalLevel { get; set; }
        public float Salary { get; set; }
        public DateTime? OnboardDate { get; set; }
        public string HRNote { get; set; }
        public string LMSInfo { get; set; }
        public bool? EmailSent { get; set; }
        public string Percentage { get; set; }
    }
}
