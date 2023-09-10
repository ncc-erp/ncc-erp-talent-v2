using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Requisitions.Dtos;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Interview.Dtos
{
    public class RequisitionInterviewerDto : RequisitionDto
    {
        public RequestCVStatus StatusRequestCV { get; set; }
        public string StatusRequestCVName { get => CommonUtils.GetEnumName<RequestCVStatus>(StatusRequestCV); }
    }
}
