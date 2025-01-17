using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Notifications.Templates.Dtos;

namespace TalentV2.Notifications.MezonWebhook.Dtos
{
    public class CandidateOfferAcceptedResult : CandidateOfferAcceptedTemplate
    {
        public string CVStatus { get; set; }
    }
}
