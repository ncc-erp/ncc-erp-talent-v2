using Abp.AutoMapper;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Categories.Dtos
{
    [AutoMapTo(typeof(SubPosition))]
    public class SubPositionDto : CategoryDto
    {
        public string ColorCode { get; set; }
        public long PositionId { get; set; }
        [ApplySearch]
        public string PositionName { get; set; }
    }
}
