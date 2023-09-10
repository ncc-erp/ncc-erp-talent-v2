using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.ExternalCVs.Dtos
{
    [AutoMapTo(typeof(ExternalCV))]
    public class CreateExternalCVDto
    {
        [Required]
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool? IsFemale { get; set; }
        public string UserTypeName { get; set; }
        public string PositionName { get; set; }
        public string CVSourceName { get; set; }
        public string BranchName { get; set; }
        public string NCCEmail { get; set; }
        public DateTime? Birthday { get; set; }
        public string ReferenceName { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string Metadata { get; set; }
        public IFormFile CV { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
