using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TalentV2.Entities.NccCVs;

namespace TalentV2.APIs.NccCVs.MyProfile.Dto
{
    [AutoMapTo(typeof(EmployeeWorkingExperience))]
    public class WorkingExperienceDto
    {
        public long? Id { get; set; }
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Position { get; set; }
        public string ProjectDescription { get; set; }
        public string Responsibility { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [Required]
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int? Order { get; set; }
        public string Technologies { get; set; }
        public bool IsChecked { get; set; }
        public long? VersionId { get; set; }
        public IEnumerable<TechOfWorkingExp> ListOfTechnologies { get; set; }
    }
    public class TechOfWorkingExp
    {
        public long TechId { get; set; }
        public string TechName { get; set; }
        public string TechVersion { get; set; }
    }
}
