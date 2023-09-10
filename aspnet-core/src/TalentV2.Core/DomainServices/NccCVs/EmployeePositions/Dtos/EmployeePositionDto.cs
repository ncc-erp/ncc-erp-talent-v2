using Abp.AutoMapper;
using NccCore.Anotations;
using System.ComponentModel.DataAnnotations;
using TalentV2.Entities.NccCVs;

namespace TalentV2.DomainServices.NccCVs.EmployeePositions.Dtos
{
    [AutoMapTo(typeof(EmployeePosition))]
    public class EmployeePositionDto
    {
        public long Id { get; set; }
        [ApplySearchAttribute]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
