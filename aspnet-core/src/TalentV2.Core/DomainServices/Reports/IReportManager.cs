using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Reports.Dtos;

namespace TalentV2.DomainServices.Reports
{
    public interface IReportManager : IDomainService
    {
        Task<OverviewHiringDto> GetOverviewHiring(DateTime fd, DateTime td, UserType? userType, long? branchId);
        Task<CVSourceStatisticDto> GetPerformanceCVSource(DateTime fd, DateTime td, UserType userType, long? branchId);
        Task<ReportEducationByBranchDto<ReportEducationHaveCVPassTestDto>> GetEducationPassTest(DateTime fd, DateTime td, long? branchId);
        Task<ReportEducationByBranchDto<ReportEducationHaveCVOnboardDto>> GetEducationInternOnboarded(DateTime fd, DateTime td, long? branchId);
    }
}
