namespace TalentV2.Constants.Enum
{
    public enum NotificationType
    {
        //from HR
        AcceptedOffer,
        RejectedOffer,
        UpdatedPersonalInfo,

        //from background worker
        ChannelNotice_InterviewRemind,
        ChannelNotice_CandidateEvaluation,
        ChannelNotice_CandidateEvaluationAndLevel,
        UserNotice_InterviewRemind,
        UserNotice_CandidateEvaluation,
        UserNotice_CandidateEvaluationAndLevel,

        CrawlCV_MessageToChannel,
        CrawlCV_MessageToUser,
    }
}
