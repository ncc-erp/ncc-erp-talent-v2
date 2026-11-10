using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TalentV2.DomainServices.AIInterviews.Dtos
{
    /// <summary>
    /// Payload sent by the AI interview bot when an interview finishes.
    /// Only the fields Talent needs are mapped; the whole "report" is kept as JSON.
    /// </summary>
    public class AIInterviewWebhookDto
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("candidate_email")]
        public string CandidateEmail { get; set; }

        [JsonProperty("candidate_name")]
        public string CandidateName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>Mezon user id of the candidate.</summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("coach_id")]
        public string CoachId { get; set; }

        [JsonProperty("coach_name")]
        public string CoachName { get; set; }

        [JsonProperty("scenario_id")]
        public string ScenarioId { get; set; }

        [JsonProperty("scenario_title")]
        public string ScenarioTitle { get; set; }

        [JsonProperty("stats")]
        public AIInterviewStatsDto Stats { get; set; }

        [JsonProperty("report")]
        public JObject Report { get; set; }
    }

    public class AIInterviewStatsDto
    {
        [JsonProperty("total_turns")]
        public int? TotalTurns { get; set; }

        [JsonProperty("candidate_turns")]
        public int? CandidateTurns { get; set; }

        [JsonProperty("total_words_spoken")]
        public int? TotalWordsSpoken { get; set; }

        [JsonProperty("duration_seconds")]
        public double? DurationSeconds { get; set; }
    }

    public class AIInterviewWebhookResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public long? ResultId { get; set; }
    }
}
