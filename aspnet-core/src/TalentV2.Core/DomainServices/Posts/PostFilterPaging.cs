using NccCore.Paging;
using System;

namespace TalentV2.DomainServices.Posts.Dtos
{
    public class PostFilterPaging : GridParam
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
