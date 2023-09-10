using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.ModelExtends;
using TalentV2.Notifications.Mail.Dtos;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class CandidateDto : UserInfo
    {
        public long Id { get; set; }
        public List<SkillCandidatePagingDto> CVSkills { get; set; }
        public List<RequisitionInfoDto> RequisitionInfos { get; set; }
        public List<MailStatusHistoryDto> MailStatusHistories { get; set; }
        public List<HistoryChangeStatusesDto> HistoryChangeStatuses { get; set; }
        public ProcessCVStatus ProcessCVStatus { get; set; }
        public DateTime? LatestModifiedTime { get; set; }
    }
    public class SkillCandidatePagingDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Level Level { get; set; }
        public LevelDto LevelInfo { get => CommonUtils.ListLevel.Where(s => s.Id == Level.GetHashCode()).Select(s => s).FirstOrDefault(); }
    }
}
