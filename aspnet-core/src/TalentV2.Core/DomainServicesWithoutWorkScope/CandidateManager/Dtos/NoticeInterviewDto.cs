using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalentV2.Constants.Enum;
using TalentV2.Utils;

namespace TalentV2.DomainServicesWithoutWorkScope.CandidateManager.Dtos
{
    public class NoticeInterviewDto
    {
        public long RequestCVId { get; set; }
        public long CVId { get; set; }
        public UserType UserType { get; set; }
        public string BranchName { get; set; }
        public string PositionName { get; set; }
        public string CandidateFulName { get; set; }
        public RequestCVStatus Status { get; set; }
        public DateTime TimeInterview { get; set; }
        public List<string> InterviewerEmails { get; set; }
        public bool? Interviewed { get; set; }
        public string GetMessageToChannel(string feUrl, bool isSchedule)
        {
            var sb = new StringBuilder();
            sb.Append(string.Join(", ", InterviewerEmails.Select(i=>CommonUtils.GetDiscordTagUser(i)).ToArray()));
            sb.Append(isSchedule?": Bạn có lịch phỏng vấn ứng viên ":": Bạn cần nhập **đánh giá** ứng viên ");
            sb.Append($"**{CandidateFulName}** [{BranchName}] **{UserType} {PositionName}**");
            sb.Append($" phỏng vấn ngày: **{DateTimeUtils.ToddMMyyyyHHmm(TimeInterview)}** \n");
            sb.Append($"{feUrl}app/candidate/{(UserType == UserType.Intern ? "intern-list" : "staff-list")}/{CVId}?userType={(int)UserType}&tab=3");
            return sb.ToString();
        }
        public string GetMessageToUser(string feUrl, bool isSchedule)
        {
            var sb = new StringBuilder();
            //sb.Append(string.Join(", ", InterviewerEmails.Select(i=>CommonUtils.GetUserNameByEmail(i)).ToArray()));
            sb.Append(isSchedule?" Bạn có lịch phỏng vấn ứng viên ":" Bạn cần nhập **đánh giá** ứng viên ");
            sb.Append($"**{CandidateFulName}** [{BranchName}] **{UserType} {PositionName}**");
            sb.Append($" phỏng vấn ngày: **{DateTimeUtils.ToddMMyyyyHHmm(TimeInterview)}** \n");
            sb.Append($"{feUrl}app/candidate/{(UserType == UserType.Intern ? "intern-list" : "staff-list")}/{CVId}?userType={(int)UserType}&tab=3");
            return sb.ToString();
        }
       
  }
}
