using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.ApplyCVs.Dtos
{
    [AutoMapTo(typeof(ApplyCV))]
    public class CreateApplyCVDto
    {
        public string Name { get; set; }
        public bool IsFemale { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PositionType { get; set; }
        public string JobTitle { get; set; }
        public string Branch { get; set; }
        public IFormFile Avatar { get; set; }
        public IFormFile AttachCV { get; set; }
        public long PostId { get; set; }
    }
}
