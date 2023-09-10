using System;
using System.Collections.Generic;
using System.Text;

namespace TalentV2.APIs.NccCVs.Technology.Dto
{
    public class GetAllTechnologiesDto
    {
        public long Id { get; set; }
        public string Technology { get; set; }
        public string Version { get; set; }
    }
}
