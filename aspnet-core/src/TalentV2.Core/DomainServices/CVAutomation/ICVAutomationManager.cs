﻿using Abp.Domain.Services;
using System.Threading.Tasks;
using TalentV2.DomainServices.CVAutomation.Dto;

namespace TalentV2.DomainServices.CVAutomation
{
    public interface ICVAutomationManager : IDomainService
    {
        Task<AutomationResult> AutoCreateInternCV();

        Task<AutomationResult> AutoCreateStaffCV();
    }
}