using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;

namespace TalentV2.DomainServices.ApplyCVs.Dtos
{
    public class ApplyCvFilterPaging : GridParam
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public UserType? UserType { get; set; }
        public long? BranchId { get; set; }
    }
}