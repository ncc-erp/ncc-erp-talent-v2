using Abp.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Reports;
using TalentV2.DomainServices.Reports.Dtos;
using TalentV2.DomainServices.Requisitions.Dtos;
using TalentV2.Entities;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class ReportAppService : TalentV2AppServiceBase
    {
        private readonly IReportManager _reportManager;
        public ReportAppService(IReportManager reportManager) 
        {
            _reportManager = reportManager;
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Reports_Overview)]
        public async Task<OverviewHiringDto> GetOverviewHiring(DateTime fd, DateTime td, UserType? userType, long? branchId)
        {
            return await _reportManager.GetOverviewHiring(fd, td, userType, branchId);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Reports_Staff_Performance)]
        public async Task<CVSourceStatisticDto> GetPerformanceStaffCVSource(DateTime fd, DateTime td, long? branchId)
        {
            return await _reportManager.GetPerformanceCVSource(fd, td, UserType.Staff, branchId);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Reports_Intern_Performance)]
        public async Task<CVSourceStatisticDto> GetPerformanceInternCVSource(DateTime fd, DateTime td, long? branchId)
        {
            return await _reportManager.GetPerformanceCVSource(fd, td, UserType.Intern, branchId);
        }
        [HttpGet]
        public async Task<ReportEducationByBranchDto<ReportEducationHaveCVPassTestDto>> GetEducationPassTest(DateTime fd, DateTime td, long? branchId)
        {
            return await _reportManager.GetEducationPassTest(fd, td, branchId);
        }
        [HttpGet]
        public async Task<ReportEducationByBranchDto<ReportEducationHaveCVOnboardDto>> GetEducationInternOnboarded(DateTime fd, DateTime td, long? branchId)
        {
            return await _reportManager.GetEducationInternOnboarded(fd,td,branchId);
        }
    }
}
