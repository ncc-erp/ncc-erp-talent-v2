using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Requisitions.Dtos
{
    public class ResultRequisition<T> where T : class
    {
        public int TotalCount { get; set; }
        public IReadOnlyList<T> Items { get; set; }
        public int TotalQuantity { get; set; }
    }
}
