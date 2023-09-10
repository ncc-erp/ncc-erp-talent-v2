using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TalentV2.APIs.NccCVs.Versions.Dto
{
    public class VersionDto : EntityDto<long>
    {
        public long EmployeeId { get; set; }
        [StringLength(50)]
        [Required]
        public string VersionName { get; set; }
        public long PositionId { get; set; }
        public long? LanguageId { get; set; }
    }
}
