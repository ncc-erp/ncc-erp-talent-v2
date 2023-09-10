using System;
using System.Collections.Generic;
using System.Text;

namespace TalentV2.APIs.NccCVs.Employee.Dto
{
    public class GetEmployeeParam
    {
        public string Name { get; set; }
        public long? PositionId { get; set; }
        public long? PositionVersId { get; set; }
        public long? BranchId { get; set; }
        public long? LanguageId { get; set; }
        public int MaxResultCount { get; set; }
        public int SkipCount { get; set; }
    }
}
