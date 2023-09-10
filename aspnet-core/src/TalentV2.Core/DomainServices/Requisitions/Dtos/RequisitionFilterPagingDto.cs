using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Requisitions.Dtos
{
    public class RequisitionFilterPagingDto : GridParam
    {
        public List<long> SkillIds { get; set; }
        public bool IsAndCondition { get; set; }
    }
}
