using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Constants.Enum
{
    public enum RequestCVStatus
    {
        AddedCV,
        ScheduledTest,
        FailedTest,
        PassedTest,
        ScheduledInterview,
        RejectedInterview,
        PassedInterview,
        FailedInterview,
        AcceptedOffer,
        RejectedOffer,
        Onboarded,
        RejectedTest,
        RejectedApply
    }
}
