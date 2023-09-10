using Abp.Domain.Services;
using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.ExternalCVs.Dtos;
using TalentV2.Entities;
using TalentV2.NccCore;

namespace TalentV2.DomainServices.ExternalCVs
{
    public interface IExternalCVManager : IDomainService
    {
        Task<long> CreateExternalCV(CreateExternalCVDto input);
        Task<ExternalCVDto> GetExternalCVById(long cvId);
        IQueryable<ExternalCVDto> IQGetAllExternalCVs();
        Task<ExternalCVDto> UpdateExternalCV(UpdateExternalCVDto input);
    }
}
