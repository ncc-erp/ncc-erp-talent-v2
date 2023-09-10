using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServices.RequestCVs.Dtos;

namespace TalentV2.DomainServices.RequestCVs
{
    public interface IRequestCVManager : IDomainService
    {
        IQueryable<CandidateOfferDto> IQGetRequestCV();
        Task<long> UpdateCandidateOffer(UpdateCandidateOfferDto input);
        Task<long> UpdateCandidateOnboard(UpdateCandidateOnboardDto input);
    }
}
