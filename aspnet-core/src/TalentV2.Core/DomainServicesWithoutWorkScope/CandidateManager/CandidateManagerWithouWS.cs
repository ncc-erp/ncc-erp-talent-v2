using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using TalentV2.Configuration;
using TalentV2.Constants.Enum;
using TalentV2.DomainServicesWithoutWorkScope.CandidateManager.Dtos;
using TalentV2.Entities;

namespace TalentV2.DomainServicesWithoutWorkScope.CandidateManager
{
    public class CandidateManagerWithouWS : BaseManagerWithoutWorkScope, ICandidateManagerWithouWS
    {
        private readonly IRepository<RequestCV, long> _repoRequestCv;

        public CandidateManagerWithouWS(IRepository<RequestCV, long> repoRequestCv)
        {
            _repoRequestCv = repoRequestCv;
        }


        public List<NoticeInterviewDto> GetNoticeInteviewInfo(DateTime now)
        {
            var minutes = int.Parse(SettingManager.GetSettingValueForApplication
                (AppSettingNames.NoticeInterviewMinutes));
            var listCvID = _repoRequestCv.GetAll()
                  .Where(x => x.InterviewTime.HasValue)
                  .Where(x => x.RequestCVInterviews != null)
                  .Where(x => x.Status == RequestCVStatus.ScheduledInterview)
                  .Where(x => x.Request.Status == StatusRequest.InProgress)
                .Select(s => new NoticeInterviewDto
                {
                    RequestCVId = s.Id,
                    TimeInterview = s.InterviewTime.Value,
                    Status = s.Status,
                    CandidateFulName = s.CV.Name,
                    PositionName = s.CV.SubPosition.Name,
                    BranchName = s.CV.Branch.Name,
                    UserType = s.CV.UserType,
                    CVId = s.CVId,
                    InterviewerEmails = s.RequestCVInterviews.Select(t => t.Interview.EmailAddress).ToList(),
                    Interviewed = s.Interviewed,
                }).ToList()
                .Where(x => x.InterviewerEmails.Count > 0)
                .Where(s => s.TimeInterview > now &&
                            s.TimeInterview.Subtract(now).TotalMinutes < minutes).ToList();
            return listCvID;
        }
        public List<NoticeInterviewDto> GetNoticeResultInteviewInfo(DateTime now)
        {
            var minutes = int.Parse(SettingManager.GetSettingValueForApplication
                (AppSettingNames.NoticeInterviewResultMinutes));
            var listCvID = _repoRequestCv.GetAll()
                  .Where(x => x.InterviewTime.HasValue)
                  .Where(x => x.RequestCVInterviews != null)
                  .Where(x => x.Status == RequestCVStatus.ScheduledInterview)
                  .Where(x => x.Request.Status == StatusRequest.InProgress)
                .Select(s => new NoticeInterviewDto
                {
                    RequestCVId = s.Id,
                    TimeInterview = s.InterviewTime.Value,
                    Status = s.Status,
                    CandidateFulName = s.CV.Name,
                    PositionName = s.CV.SubPosition.Name,
                    BranchName = s.CV.Branch.Name,
                    UserType = s.CV.UserType,
                    CVId = s.CVId,
                    InterviewerEmails = s.RequestCVInterviews.Select(t => t.Interview.EmailAddress).ToList(),
                    Interviewed = s.Interviewed,
                }).ToList()
                .Where(x => x.InterviewerEmails.Count > 0)
                .Where(s => now.Subtract(s.TimeInterview).TotalMinutes > minutes)
                .ToList();
            return listCvID;
        }
    }
}
