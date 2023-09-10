using System;
using System.Collections.Generic;
using System.Text;

namespace TalentV2.APIs.NccCVs.Employee.Dto
{
    public class EmployeeDto
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public long? PositionId { get; set; }
        public long? BranchId { get; set; }
        public string Surname { get; set; }
        public string FullName { get; set; }
    }
}
