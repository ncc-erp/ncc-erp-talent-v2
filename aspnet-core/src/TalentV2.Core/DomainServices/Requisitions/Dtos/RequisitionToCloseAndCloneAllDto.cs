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
    public class RequisitionToCloseAndCloneAllDto
    {
        public long Id { get; set; }
        public string BranchName { get; set; }
        public Priority Priority { get; set; }
        public string PriorityName { get => CommonUtils.GetEnumName(Priority); }
        public string SubPositionName { get; set; }
        public DateTime? TimeNeed { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public List<SkillDto> Skills { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
