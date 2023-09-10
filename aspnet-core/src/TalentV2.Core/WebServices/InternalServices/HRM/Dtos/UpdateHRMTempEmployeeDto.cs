using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;

namespace TalentV2.WebServices.InternalServices.HRM.Dtos
{
    public class UpdateHRMTempEmployeeDto
    {
        public RequestCVStatus Status { get; set; }
        public Level? FinalLevel { get; set; }
        public DateTime? OnboardDate { get; set; }
        public float Salary { get; set; }
        public List<string> SkillNames { get; set; }
        public string SkillStr => String.Join(", ", SkillNames);
        public string NCCEmail { get; set; }
        public string BranchName { get; set; }
        public UserType UserType { get; set; }
        public string PersonalEmail { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public bool IsFemale { get; set; }
        public string PositionName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
