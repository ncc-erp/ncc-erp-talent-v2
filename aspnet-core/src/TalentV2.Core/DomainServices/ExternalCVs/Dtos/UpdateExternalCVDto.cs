using Abp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using TalentV2.Entities;

namespace TalentV2.DomainServices.ExternalCVs.Dtos
{
    [AutoMapTo(typeof(ExternalCV))]
    public class UpdateExternalCVDto
    {
        [Required]
        public string ExternalId { get; set; }
        [Required]
        public string CVSourceName { get; set; }
        [Required]
        public string Metadata { get; set; }
    }
}
