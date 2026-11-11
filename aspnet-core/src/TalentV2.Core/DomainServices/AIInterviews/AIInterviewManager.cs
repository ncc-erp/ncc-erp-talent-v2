using Abp.Runtime.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.AIInterviews.Dtos;
using TalentV2.Entities;
using TalentV2.Utils;
using TalentV2.WebServices.ExternalServices.Komu;

namespace TalentV2.DomainServices.AIInterviews
{
    public class AIInterviewManager : BaseManager, IAIInterviewManager
    {
        private const string WordsBreakdownKey = "words_breakdown";

        private readonly KomuService _komuService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIInterviewManager> _logger;

        public AIInterviewManager(
            KomuService komuService,
            IConfiguration configuration,
            ILogger<AIInterviewManager> logger)
        {
            _komuService = komuService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AIInterviewWebhookResultDto> ReceiveResult(AIInterviewWebhookDto input)
        {
            Validate(input);

            var email = input.CandidateEmail.Trim().ToLower();
            var sessionId = input.SessionId.Trim();

            var existedId = await WorkScope.GetAll<AIInterviewResult>()
                .Where(x => x.SessionId == sessionId)
                .Select(x => (long?)x.Id)
                .FirstOrDefaultAsync();
            if (existedId.HasValue)
            {
                _logger.LogInformation("[AIInterview] Duplicate session ignored; no DMs sent. SessionId: {SessionId}, ResultId: {ResultId}, TenantId: {TenantId}",
                    sessionId, existedId, AbpSession.TenantId);
                return new AIInterviewWebhookResultDto
                {
                    Success = true,
                    Message = "Session already received",
                    ResultId = existedId
                };
            }

            var cv = await WorkScope.GetAll<CV>()
                .Where(c => c.Email.ToLower() == email && c.isClone != true)
                .OrderByDescending(c => c.LastModificationTime ?? c.CreationTime)
                .Select(c => new CandidateInfo
                {
                    CVId = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    UserType = c.UserType,
                    HREmail = c.LastModifierUser != null ? c.LastModifierUser.EmailAddress : c.CreatorUser.EmailAddress,
                })
                .FirstOrDefaultAsync();

            var entity = MapToEntity(input, sessionId, email);

            if (cv == null)
            {
                entity.ProcessStatus = AIInterviewProcessStatus.CandidateNotFound;
                entity.Note = $"No CV found with email {email}";
                var notFoundId = await WorkScope.InsertAndGetIdAsync(entity);
                _logger.LogWarning("[AIInterview] Result inserted without a matching CV; no DMs sent. SessionId: {SessionId}, ResultId: {ResultId}, TenantId: {TenantId}, ProcessStatus: {ProcessStatus}",
                    sessionId, notFoundId, AbpSession.TenantId, entity.ProcessStatus);
                return new AIInterviewWebhookResultDto
                {
                    Success = true,
                    Message = entity.Note,
                    ResultId = notFoundId
                };
            }

            var requestCv = await FindRequestCV(cv.CVId, entity.InterviewedAt);
            entity.CVId = cv.CVId;
            entity.RequestCVId = requestCv?.Id;
            entity.ProcessStatus = AIInterviewProcessStatus.Matched;

            if (requestCv != null)
            {
                // Only mark that the interview happened, HR still decides the status manually.
                requestCv.Interviewed = true;
                await WorkScope.UpdateAsync(requestCv);
            }

            var resultId = await WorkScope.InsertAndGetIdAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            _logger.LogInformation("[AIInterview] Result saved; starting notifications. SessionId: {SessionId}, ResultId: {ResultId}, TenantId: {TenantId}, CVId: {CVId}, RequestCVId: {RequestCVId}",
                sessionId, resultId, AbpSession.TenantId, entity.CVId, entity.RequestCVId);

            var errors = await Notify(entity, cv);
            if (errors.Any())
            {
                entity.ProcessStatus = AIInterviewProcessStatus.NotifyFailed;
                entity.Note = Truncate(string.Join("; ", errors), 2000);
                await WorkScope.UpdateAsync(entity);
            }

            _logger.LogInformation("[AIInterview] Result processed. SessionId: {SessionId}, ResultId: {ResultId}, ProcessStatus: {ProcessStatus}, NotificationErrors: {NotificationErrors}",
                sessionId, resultId, entity.ProcessStatus, errors.Count);

            return new AIInterviewWebhookResultDto
            {
                Success = true,
                Message = errors.Any() ? "Saved, but some notifications failed" : "Saved",
                ResultId = resultId
            };
        }

        public async Task<List<EnglishInterviewResultDto>> GetResultsByCVId(long cvId)
        {
            var results = await WorkScope.GetAll<AIInterviewResult>()
                .AsNoTracking()
                .Where(x => x.CVId == cvId)
                .OrderByDescending(x => x.InterviewedAt ?? x.CreationTime)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            return results.Select(x => new EnglishInterviewResultDto
            {
                Id = x.Id,
                SessionId = x.SessionId,
                ScenarioTitle = x.ScenarioTitle,
                CoachName = x.CoachName,
                InterviewedAt = x.InterviewedAt,
                DurationSeconds = x.DurationSeconds,
                OverallScore = x.OverallScore,
                CefrLevel = x.CefrLevel,
                PronunciationScore = x.PronunciationScore,
                FluencyScore = x.FluencyScore,
                VocabularyScore = x.VocabularyScore,
                GrammarScore = x.GrammarScore,
                DomainScore = x.DomainScore,
                IeltsBand = x.IeltsBand,
                ToeflScore = x.ToeflScore,
                Report = string.IsNullOrWhiteSpace(x.Report) ? null : JObject.Parse(x.Report)
            }).ToList();
        }

        #region Matching

        private async Task<RequestCV> FindRequestCV(long cvId, DateTime? interviewedAt)
        {
            var requestCvs = await WorkScope.GetAll<RequestCV>()
                .Where(x => x.CVId == cvId)
                .ToListAsync();
            if (!requestCvs.Any())
                return null;

            var reference = interviewedAt ?? DateTimeUtils.GetNow();
            var scheduled = requestCvs
                .Where(x => x.Status == RequestCVStatus.ScheduledInterview)
                .OrderBy(x => x.InterviewTime.HasValue ? Math.Abs((x.InterviewTime.Value - reference).TotalMinutes) : double.MaxValue)
                .FirstOrDefault();

            return scheduled ?? requestCvs.OrderByDescending(x => x.CreationTime).First();
        }

        #endregion

        #region Notify

        private async Task<List<string>> Notify(AIInterviewResult result, CandidateInfo cv)
        {
            var errors = new List<string>();
            var cvLink = BuildCVLink(cv);

            if (!string.IsNullOrWhiteSpace(result.CandidateMezonUserId))
            {
                await TrySend(errors, "Candidate Komu DM", result, () =>
                    _komuService.SendMessageToUserByIdAsync(result.CandidateMezonUserId, BuildCandidateMessage(result, cv)));
            }
            else
            {
                errors.Add("Candidate Komu DM: user_id is missing");
                _logger.LogWarning("[AIInterview] Candidate Komu DM skipped: user_id is missing. SessionId: {SessionId}, ResultId: {ResultId}",
                    result.SessionId, result.Id);
            }

            if (string.IsNullOrEmpty(cv.HREmail))
            {
                errors.Add("No HR in charge found for CV");
                _logger.LogWarning("[AIInterview] HR Komu DM skipped: no HR found. SessionId: {SessionId}, ResultId: {ResultId}",
                    result.SessionId, result.Id);
                return errors;
            }

            await TrySend(errors, "HR Komu DM", result, () =>
                _komuService.SendMessageToUserAsync(CommonUtils.GetUserNameByEmail(cv.HREmail), BuildHRMessage(result, cv, cvLink)));

            return errors;
        }

        private async Task TrySend(List<string> errors, string name, AIInterviewResult result, Func<Task> action)
        {
            try
            {
                await action();
                _logger.LogInformation("[AIInterview] {Notification} accepted by Komu. SessionId: {SessionId}, ResultId: {ResultId}",
                    name, result.SessionId, result.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AIInterview] {Notification} failed. SessionId: {SessionId}, ResultId: {ResultId}",
                    name, result.SessionId, result.Id);
                errors.Add($"{name}: {ex.Message}");
            }
        }

        private static string BuildCandidateMessage(AIInterviewResult result, CandidateInfo cv)
        {
            return $"Chào {cv.Name}, cảm ơn bạn đã tham gia phỏng vấn **{result.ScenarioTitle}**.\n"
                + $"Điểm tổng: **{FormatNumber(result.OverallScore)}** - CEFR: **{result.CefrLevel}**\n"
                + "Bộ phận Nhân sự NCC sẽ liên hệ với bạn về các bước tiếp theo.";
        }

        private static string BuildHRMessage(AIInterviewResult result, CandidateInfo cv, string cvLink)
        {
            return $"[English Interview] Ứng viên **{cv.Name}** ({cv.Email}) đã hoàn thành phỏng vấn tiếng Anh.\n"
                + $"Overall: **{FormatNumber(result.OverallScore)}** - CEFR: **{result.CefrLevel}**\n"
                + $"Xem chi tiết tại tab EnglishInterview trên hồ sơ Talent: {cvLink}";
        }

        private string BuildCVLink(CandidateInfo cv)
        {
            var feUrl = _configuration.GetValue<string>("App:ClientRootAddress") ?? "";
            if (!feUrl.EndsWith("/")) feUrl += "/";
            var list = cv.UserType == UserType.Intern ? "intern-list" : "staff-list";
            return $"{feUrl}app/candidate/{list}/{cv.CVId}?userType={(int)cv.UserType}&tab=5";
        }

        #endregion

        #region Mapping helpers

        private static void Validate(AIInterviewWebhookDto input)
        {
            var errors = new List<ValidationResult>();
            if (input == null)
                errors.Add(new ValidationResult("Body is required"));
            else
            {
                if (string.IsNullOrWhiteSpace(input.CandidateEmail))
                    errors.Add(new ValidationResult("candidate_email is required", new[] { "candidate_email" }));
                if (string.IsNullOrWhiteSpace(input.SessionId))
                    errors.Add(new ValidationResult("session_id is required", new[] { "session_id" }));
            }
            if (errors.Any())
                throw new AbpValidationException("Invalid AI interview payload", errors);
        }

        private static AIInterviewResult MapToEntity(AIInterviewWebhookDto input, string sessionId, string email)
        {
            var report = input.Report;
            JObject reportToStore = null;
            if (report != null)
            {
                reportToStore = (JObject)report.DeepClone();
                reportToStore.Remove(WordsBreakdownKey);
            }

            return new AIInterviewResult
            {
                SessionId = Truncate(sessionId, 255),
                CandidateEmail = Truncate(email, 255),
                CandidateMezonUserId = Truncate(input.UserId, 100),
                CandidateName = Truncate(input.CandidateName, 255),
                CoachName = Truncate(input.CoachName, 255),
                ScenarioId = Truncate(input.ScenarioId, 255),
                ScenarioTitle = Truncate(input.ScenarioTitle, 500),
                InterviewedAt = ParseTimestamp(input.Timestamp),
                DurationSeconds = input.Stats?.DurationSeconds,
                OverallScore = GetDouble(report, "overall_score"),
                CefrLevel = Truncate(GetString(report, "cefr_level"), 10),
                PronunciationScore = GetDouble(report, "pronunciation_score"),
                FluencyScore = GetDouble(report, "fluency_score"),
                VocabularyScore = GetDouble(report, "vocabulary_score"),
                GrammarScore = GetDouble(report, "grammar_score"),
                DomainScore = GetDouble(report, "domain_score"),
                IeltsBand = GetDouble(report, "ielts_speaking_band"),
                ToeflScore = GetDouble(report, "toefl_speaking_score"),
                Report = reportToStore?.ToString(Formatting.None),
            };
        }

        private static DateTime? ParseTimestamp(string timestamp)
        {
            if (string.IsNullOrWhiteSpace(timestamp))
                return null;
            if (DateTimeOffset.TryParse(timestamp, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var value))
                return value.ToLocalTime().DateTime;
            return null;
        }

        private static double? GetDouble(JObject obj, string key)
        {
            var token = obj?[key];
            if (token == null || token.Type == JTokenType.Null)
                return null;
            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
                return token.Value<double>();
            return double.TryParse(token.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null;
        }

        private static string GetString(JObject obj, string key)
        {
            var token = obj?[key];
            return token == null || token.Type == JTokenType.Null ? null : token.ToString();
        }

        private static string FormatNumber(double? value) =>
            value.HasValue ? value.Value.ToString("0.##", CultureInfo.InvariantCulture) : "";

        private static string Truncate(string value, int max) =>
            string.IsNullOrEmpty(value) || value.Length <= max ? value : value.Substring(0, max);

        #endregion

        private class CandidateInfo
        {
            public long CVId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public UserType UserType { get; set; }
            public string HREmail { get; set; }
        }
    }
}
