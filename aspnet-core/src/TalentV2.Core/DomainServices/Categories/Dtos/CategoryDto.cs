using Abp.AutoMapper;
using TalentV2.Entities;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Categories.Dtos
{
    public class CategoryDto
    {
        public long Id { get; set; }
        [ApplySearchAttribute]
        [Required]
        public string Name { get; set; }
    }

    [AutoMapTo(typeof(Position))]
    public class PositionDto : CategoryDto 
    {
        [MaxLength(20)]
        public string Code { get; set; }
        [MaxLength(20)]
        public string ColorCode { get; set; }
    }

    public class MailTemplateDto : CategoryDto
    {
        public string Version { get; set; }
    }
}
