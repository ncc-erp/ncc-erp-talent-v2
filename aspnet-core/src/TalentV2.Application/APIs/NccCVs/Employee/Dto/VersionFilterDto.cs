using System;
using System.Collections.Generic;
using System.Text;

namespace TalentV2.APIs.NccCVs.Employee.Dto
{
    public class VersionFilterDto
    {
        public string VersionName { get; set; }
        public long? VersionPositionId { get; set; }
        public long? VersionLanguageId { get; set; }
    }
}
