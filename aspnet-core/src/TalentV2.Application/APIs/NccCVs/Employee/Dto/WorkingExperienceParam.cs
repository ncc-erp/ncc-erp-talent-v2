using System;
using System.Collections.Generic;
using System.Text;

namespace TalentV2.APIs.NccCVs.Employee.Dto
{
    public class WorkingExperienceParam
    {
        public string Technologies { get; set; }
        public string Positions { get; set; }
        public string ProjectName { get; set; }
        public string EmployeeName { get; set; }
        public bool IsIncludeVers { get; set; }
        public int MaxResultCount { get; set; }
        public int SkipCount { get; set; }
    }
}
