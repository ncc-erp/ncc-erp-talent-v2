using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServicesWithoutWorkScope.CandidateManager.Dtos;

namespace TalentV2.Notifications.MezonWebhook.Dtos
{
    public class NoticeInterviewResult : NoticeInterviewDto
    {
        public string CVLink { get; set; }
    }
}
