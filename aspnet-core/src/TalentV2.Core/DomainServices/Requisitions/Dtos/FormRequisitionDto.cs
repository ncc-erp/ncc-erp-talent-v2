using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Requisitions.Dtos
{
    public class FormRequisitionDto
    {
        [Required]
        public UserType UserType { get; set; }
        [Required]
        public long SubPositionId { get; set; }
        [Required]
        public Priority Priority { get; set; }
        [Required]
        public List<long> SkillIds { get; set; }
        [Required]
        public long BranchId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [MaxLength(1000)]
        public string Note { get; set; }
    }
    [AutoMapFrom(typeof(Request))]
    public class DetailRequisitionDto : FormRequisitionDto
    {
        public Level Level { get; set; }
        public DateTime TimeNeed { get; set; }
    }
    #region Staff
    [AutoMapTo(typeof(Request))]
    public class CreateRequisitionStaffDto : FormRequisitionDto
    {
        [Required]
        public Level Level { get; set; }
        [Required]
        public DateTime TimeNeed { get; set; }
    }
    [AutoMapTo(typeof (Request))]
    public class UpdateRequisitionStaffDto : FormRequisitionDto
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public Level Level { get; set; }
        [Required]
        public DateTime TimeNeed { get; set; }
    }
    #endregion

    #region Intern
    [AutoMapTo(typeof(Request))]
    public class CreateRequisitionInternDto : FormRequisitionDto 
    {
        public DateTime TimeNeed { get; set; }
    }
    [AutoMapTo(typeof(Request))]
    public class UpdateRequisitionInternDto : FormRequisitionDto
    {
        [Required(ErrorMessage = "Id is required.")]
        public long Id { get; set; }
        public DateTime TimeNeed { get; set; }
    }
    #endregion
}
