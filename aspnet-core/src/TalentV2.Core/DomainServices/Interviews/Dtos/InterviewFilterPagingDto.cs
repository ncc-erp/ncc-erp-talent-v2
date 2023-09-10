using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Interviews.Dtos
{
    public class InterviewFilterPagingDto : GridParam
    {
        public List<long> InterviewerIds { get; set; }
        public bool IsAndCondition { get; set; }
    }
}
