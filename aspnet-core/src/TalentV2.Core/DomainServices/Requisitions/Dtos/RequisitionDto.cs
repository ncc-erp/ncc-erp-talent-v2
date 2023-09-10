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
    public class RequisitionDto : CreateUpdateAudit, IIsProjectTool
    {
        public long Id { get; set; }
        public Priority Priority { get; set; }
        public string PriorityName { get => CommonUtils.GetEnumName(Priority); }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public Level Level { get; set; }
        public LevelDto LevelInfo { get => CommonUtils.ListLevel.Where(s => s.Id == Level.GetHashCode()).Select(s => s).FirstOrDefault(); }
        public List<SkillDto> Skills { get; set; }
        public DateTime? TimeNeed { get; set; }
        public StatusRequest Status { get; set; }
        public string StatusName { get => CommonUtils.GetEnumName(Status); }
        public int Quantity { get; set; }
        public DateTime? TimeClose { get; set; }
        public int QuantityOnboard { get; set; }
        public int QuantityFail { get; set; }
        public int TotalCandidateApply { get; set; }
        public UserType UserType { get; set; }
        public string UserTypeName { get => CommonUtils.GetEnumName(UserType); }
        public string Note { get; set; }
        public bool IsProjectTool {
            get
            {
                if (ProjectToolRequestId.HasValue)
                    return true;
                return false;
            }
        }
        public long? ProjectToolRequestId { get; set; }
    }
}
