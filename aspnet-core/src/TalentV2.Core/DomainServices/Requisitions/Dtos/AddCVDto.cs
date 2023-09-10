using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.Requisitions.Dtos
{
    public class AddCVDto
    {
        public long RequestId { get; set; }
        public List<long> CVIds { get; set; }
        public long? OldRequestId { get; set; }
        public long? SubPositionId { get; set; }
        public UserType? UserType { get; set; }
    }
}
