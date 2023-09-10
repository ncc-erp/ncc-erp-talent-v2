using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.ModelExtends;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Requisitions.Dtos
{
    public class InputRequisitionToCloseAndCloneAllDto
    {
        public List<InputRequisitionToCloseAndCloneDto> ListRequisitionIntern { get; set; }
    }
    public class InputRequisitionToCloseAndCloneDto
    {
        public long RequestId { get; set; }
        public DateTime? TimeNeed { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
    }
}
