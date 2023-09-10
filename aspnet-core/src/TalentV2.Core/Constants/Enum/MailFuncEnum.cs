using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Constants.Enum
{
    public enum MailFuncEnum : byte
    {
        FailedCV,
        ScheduledTest,
        FailedTest,
        ScheduledInterview,
        FailedInterview,
        AcceptedOfferInternship,
        AcceptedOfferJob,
        RejectedOffer,
    }
}
