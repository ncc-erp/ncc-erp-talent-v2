using Newtonsoft.Json.Linq;
using System;

namespace TalentV2.DomainServices.AIInterviews.Dtos
{
    public class EnglishInterviewResultDto
    {
        public long Id { get; set; }
        public string SessionId { get; set; }
        public string ScenarioTitle { get; set; }
        public string CoachName { get; set; }
        public DateTime? InterviewedAt { get; set; }
        public double? DurationSeconds { get; set; }
        public double? OverallScore { get; set; }
        public string CefrLevel { get; set; }
        public double? PronunciationScore { get; set; }
        public double? FluencyScore { get; set; }
        public double? VocabularyScore { get; set; }
        public double? GrammarScore { get; set; }
        public double? DomainScore { get; set; }
        public double? IeltsBand { get; set; }
        public double? ToeflScore { get; set; }
        public JObject Report { get; set; }
    }
}
