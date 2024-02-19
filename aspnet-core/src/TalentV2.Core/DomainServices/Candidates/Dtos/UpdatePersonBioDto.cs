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
    [AutoMapTo(typeof(CV))]
    public class UpdatePersonBioDto 
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public UserType UserType { get; set; }
        public long SubPositionId { get; set; }
        public long? CVSourceId { get; set; }
        public long BranchId { get; set; }
        public DateTime? Birthday { get; set; }
        public long? ReferenceId { get; set; }
        public bool IsFemale { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public CVStatus CVStatus { get; set; }
        public long? CreatorUserId { get; set; }
    }
}
