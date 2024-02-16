using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.ModelExtends;
using TalentV2.Notifications.Mail.Dtos;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class PersonBioDto : UserInfo
    {
        public long Id { get; set; }
        public DateTime? DOB { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public long? CVSourceId { get; set; }
        public long? ReferenceId { get; set; }
        public MailDetailDto MailDetail { get; set; }
        public string CandidateLanguage { get; set; }
    }
    public class MailDetailDto
    {
        public bool IsAllowSendMail { get; set; }
        public bool IsSentMailStatus { get; set; }
        public List<MailStatusHistoryDto> MailStatusHistories { get; set; }
    }
}
