using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServices.Interview.Dtos;
using TalentV2.DomainServices.Requisitions.Dtos;

namespace TalentV2.DomainServices.Interviews
{
    public interface IInterviewManager : IDomainService
    {
        IQueryable<CVRequisitionDto> IQCVsByInterviewer();
        IQueryable<long> IQGetInterviewHaveAnyInterviewerId(List<long> interviewIds);
    }
}
