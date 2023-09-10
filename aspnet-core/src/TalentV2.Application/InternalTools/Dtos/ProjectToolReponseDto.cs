using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.InternalTools.Dtos
{
    public class ProjectToolReponseDto
    {
        public bool Success { get; set; }
        public string Result { get; set; }
    }
    public class CloseRequestByProjectToolDto
    {
        public long ResourceRequestId { get; set; }
    }
}
