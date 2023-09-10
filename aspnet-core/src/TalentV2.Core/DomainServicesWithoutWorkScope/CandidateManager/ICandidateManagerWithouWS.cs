using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServicesWithoutWorkScope.CandidateManager.Dtos;

namespace TalentV2.DomainServicesWithoutWorkScope.CandidateManager
{
    public interface ICandidateManagerWithouWS : IDomainService
    {
        List<NoticeInterviewDto> GetNoticeInteviewInfo(DateTime now);
        List<NoticeInterviewDto> GetNoticeResultInteviewInfo(DateTime now);
    }
}
