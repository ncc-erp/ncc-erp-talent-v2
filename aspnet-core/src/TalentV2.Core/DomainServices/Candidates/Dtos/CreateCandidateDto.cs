using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    [AutoMapTo(typeof(CV))]
    public class CreateCandidateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public UserType UserType { get; set; }
        public long SubPositionId { get; set; }
        public long? CVSourceId { get; set; }
        public long BranchId { get; set; }
        public string NCCEmail { get; set; }
        public DateTime? Birthday { get; set; }
        public long? ReferenceId { get; set; }
        public bool IsFemale { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public CVStatus CVStatus { get; set; }
        public IFormFile CV { get; set; }
        public IFormFile Avatar { get; set; }
        public string LinkCv { get; set; }  
        public string AvatarCv { get; set; }
        public long? ApplyId { get; set; }
        public string CandidateLanguage { get; set; }
    }
}
