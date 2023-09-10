using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Castle.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TalentV2.Configuration;
using TalentV2.DomainServicesWithoutWorkScope.CandidateManager;
using TalentV2.DomainServicesWithoutWorkScope.CandidateManager.Dtos;
using TalentV2.Utils;
using TalentV2.WebServices.ExternalServices.Komu;

namespace TalentV2.BackgroundWorker
{
    public class NoticeInterviewWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        protected readonly KomuService _komuService;
        protected readonly ICandidateManagerWithouWS _candidateManagerWithouWS;
        private readonly IConfiguration _configuration;
        protected Dictionary<long, byte> _dicCVIdToNotifiedCount;
        protected DateTime _today;
        private byte MAX_SENT_COUNT = 2;

        public NoticeInterviewWorker(AbpTimer timer
            , KomuService komuService
            , ICandidateManagerWithouWS candidateManagerWithouWS
            , IConfiguration configuration) : base(timer)
        {
            _komuService = komuService;
            _candidateManagerWithouWS = candidateManagerWithouWS;
            _configuration = configuration;
            _today = DateTimeUtils.GetNow().Date;
            _dicCVIdToNotifiedCount = new();
            //Timer.RunOnStart = true;
            Timer.Period = 1000 * 60 * 10; // repeat each 10 minutes
            //Timer.Period = 5000;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            Logger.Info("DoWork start");
            DateTime now = DateTimeUtils.GetNow();
            if (now.DayOfWeek == DayOfWeek.Sunday || now.DayOfWeek == DayOfWeek.Saturday)
            {
                Logger.Info("Today is DayOff => stop");
                return;
            }
            int startAtHour, endAtHour;
            var canStart = int.TryParse(SettingManager.GetSettingValueForApplication(AppSettingNames.NoticeInterviewStartAtHour), out startAtHour);
            var canEnd = int.TryParse(SettingManager.GetSettingValueForApplication(AppSettingNames.NoticeInterviewEndAtHour), out endAtHour);
            if ((canStart && canEnd) == false )
            {
                startAtHour = 10;
                endAtHour = 17;
            }
            if (!now.Hour.IsBetween(startAtHour , endAtHour ))
            {
               Logger.Info($"now.Hour is not  between ({startAtHour } and {endAtHour })=> stop");
                return;
            }
            var listInterviewer = _candidateManagerWithouWS.GetNoticeInteviewInfo(now);
            InitOrUpdateDic(listInterviewer, now);
            if (now.Date != _today)
            {
                _today = now.Date;
            }

            Notify(listInterviewer);
        }
        void Notify(List<NoticeInterviewDto> listInterviewer)
        {
            var channelId = SettingManager.GetSettingValueForApplication(AppSettingNames.NoticeInterviewResultChannel);
            var isToChannel = SettingManager.GetSettingValueForApplication(AppSettingNames.IsNoticeInterviewViaChannel);
            var feUrl = _configuration.GetValue<string>($"App:ClientRootAddress");
            foreach (var item in listInterviewer)
            {
                var sentCount = _dicCVIdToNotifiedCount[item.RequestCVId];
                if (sentCount >= MAX_SENT_COUNT)
                {
                    continue;
                }
                if (isToChannel == "true")
                {
                    _komuService.NotifyToChannel(item.GetMessageToChannel(feUrl, true), channelId);
                }
                else
                {
                    item.InterviewerEmails.ForEach(i =>
                    _komuService.SendMessageToUser(CommonUtils.GetUserNameByEmail(i),
                    item.GetMessageToUser(feUrl, true)));
                }
                _dicCVIdToNotifiedCount[item.RequestCVId] += 1;
            }
        }

        protected void InitOrUpdateDic(List<NoticeInterviewDto> listInterviewer, DateTime now)
        {
            if (now.Date != _today)
            {
                _dicCVIdToNotifiedCount.Clear();
                listInterviewer.ForEach(i => _dicCVIdToNotifiedCount.Add(i.RequestCVId, 0));
            }
            else
            {
                foreach (var i in listInterviewer)
                {
                    if (!_dicCVIdToNotifiedCount.ContainsKey(i.RequestCVId))
                    {
                        _dicCVIdToNotifiedCount.Add(i.RequestCVId, 0);
                    }
                }
            }
        }
    }

}