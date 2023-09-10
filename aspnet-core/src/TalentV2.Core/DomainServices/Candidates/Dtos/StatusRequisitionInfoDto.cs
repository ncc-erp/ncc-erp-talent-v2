using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class StatusRequisitionInfoDto
    {
        public StatusRequest Status { get; set; }
        public string StatusName { get => CommonUtils.GetEnumName(Status); }
        public DateTime? LastTimeModifiedStatus { get; set; }
    }
}
