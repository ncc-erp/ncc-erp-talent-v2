using System;
using System.Collections.Generic;
using System.Text;
using TalentV2.Entities.NccCVs;

namespace TalentV2.APIs.NccCVs.Project.Dto
{
    public class GetProjectParam
    {
        public string Name { get; set; }
        public string Technology { get; set; }
        public string TechVersion { get; set; }
        public ProjectType? Type { get; set; }
        public int MaxResultCount { get; set; }
        public int SkipCount { get; set; }
    }
}
