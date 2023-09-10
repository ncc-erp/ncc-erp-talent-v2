using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Requisitions.Dtos
{
    public class CVAvailableCloneDto
    {
        public long RequestCVId { get; set; }
        public long CVId { get; set; }
        public long RequestId { get; set; }
    }
}
