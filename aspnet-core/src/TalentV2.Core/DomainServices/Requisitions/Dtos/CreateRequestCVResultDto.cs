using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Requisitions.Dtos
{
    public class CreateRequestCVResultDto
    {
        public List<long> CVIdsSuccess { get; set; } = new List<long>();
        public List<long> CVIdsFail { get; set; } = new List<long>();
    }
}
