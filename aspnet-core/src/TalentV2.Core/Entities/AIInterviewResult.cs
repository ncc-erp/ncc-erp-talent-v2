using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    /// <summary>
    /// Result pushed by the AI interview bot through the public webhook.
    /// </summary>
    [Index(nameof(SessionId), IsUnique = true)]
    [Index(nameof(CandidateEmail))]
    public class AIInterviewResult : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(255)]
        public string SessionId { get; set; }

        [MaxLength(255)]
        public string CandidateEmail { get; set; }

        /// <summary>Mezon user id of the candidate (field "user_id" from the bot).</summary>
        [MaxLength(100)]
        public string CandidateMezonUserId { get; set; }

        [MaxLength(255)]
        public string CandidateName { get; set; }

        public long? CVId { get; set; }
        [ForeignKey(nameof(CVId))]
        public CV CV { get; set; }

        public long? RequestCVId { get; set; }
        [ForeignKey(nameof(RequestCVId))]
        public RequestCV RequestCV { get; set; }

        [MaxLength(255)]
        public string CoachName { get; set; }
        [MaxLength(255)]
        public string ScenarioId { get; set; }
        [MaxLength(500)]
        public string ScenarioTitle { get; set; }

        public DateTime? InterviewedAt { get; set; }
        public double? DurationSeconds { get; set; }

        public double? OverallScore { get; set; }
        [MaxLength(10)]
        public string CefrLevel { get; set; }
        public double? PronunciationScore { get; set; }
        public double? FluencyScore { get; set; }
        public double? VocabularyScore { get; set; }
        public double? GrammarScore { get; set; }
        public double? DomainScore { get; set; }
        public double? IeltsBand { get; set; }
        public double? ToeflScore { get; set; }

        /// <summary>"report" object from the bot (without words_breakdown).</summary>
        [Column(TypeName = "jsonb")]
        public string Report { get; set; }

        public AIInterviewProcessStatus ProcessStatus { get; set; }

        [MaxLength(2000)]
        public string Note { get; set; }
    }
}
