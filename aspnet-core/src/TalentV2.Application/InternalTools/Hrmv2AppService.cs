using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;
using TalentV2.Configuration;
using TalentV2.DomainServices.Candidates;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.Entities;
using TalentV2.InternalTools.Dtos;
using TalentV2.Ncc;
using TalentV2.NccCore;
using TalentV2.Utils;

namespace TalentV2.InternalTools
{
    public class Hrmv2AppService : TalentV2AppServiceBase
    {
        private readonly UserManager _userManager;
        private readonly IWorkScope _ws;
        private readonly ICandidateManager _candidateManager;

        public Hrmv2AppService(UserManager userManager, ICandidateManager candidateManager, IWorkScope ws)
        {
            _userManager = userManager;
            _ws = ws;
            _candidateManager = candidateManager;
        }
        [HttpPost]
        [NccAuth]
        public async Task CreateUserFromHRM(CreateAndUpdateUserFromHrmv2Dto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var user = new User
                {
                    UserName = input.EmailAddress.Replace("@ncc.asia", ""),
                    Name = input.Name,
                    Surname = input.Surname,
                    EmailAddress = input.EmailAddress
                };

                user.Password = PasswordUtils.GeneratePassword(8, true);
                user.TenantId = AbpSession.TenantId;
                user.IsEmailConfirmed = true;

                await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

                CheckErrors(await _userManager.CreateAsync(user, user.Password));

                CheckErrors(await _userManager.SetRolesAsync(user, new string[] { StaticRoleNames.Tenants.BasicUser }));

                CurrentUnitOfWork.SaveChanges();
            }
        }
        [HttpPost]
        [NccAuth]
        public async Task UpdateUserFromHRM(CreateAndUpdateUserFromHrmv2Dto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var user = await _ws.GetAll<User>().Where(s => s.EmailAddress == input.EmailAddress).FirstOrDefaultAsync();

                user.Name = input.Name;
                user.Surname = input.Surname;

                CheckErrors(await _userManager.UpdateAsync(user));

                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task UpdateUserStatus(string emailAddress, bool isActive)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var user = await _ws.GetAll<User>().Where(s => s.EmailAddress == emailAddress).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new UserFriendlyException("Can't found user with the same email with HRM Tool");
                }
                user.IsActive = isActive;

                CheckErrors(await _userManager.UpdateAsync(user));

                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
        [HttpPost]
        [NccAuth]
        public async Task ConfirmUserQuit(UpdateUserStatusFromHRMv2Dto input)
        {
           
            await UpdateUserStatus(input.EmailAddress, false);
        }

        [HttpPost]
        [NccAuth]
        public async Task ConfirmUserPause(UpdateUserStatusFromHRMv2Dto input)
        {
            await UpdateUserStatus(input.EmailAddress, true);
        }

        [HttpPost]
        [NccAuth]
        public async Task ConfirmUserMaternityLeave(UpdateUserStatusFromHRMv2Dto input)
        {
            await UpdateUserStatus(input.EmailAddress, true);
        }

        [HttpPost]
        [NccAuth]
        public async Task ConfirmUserBackToWork(UpdateUserStatusFromHRMv2Dto input)
        {
            await UpdateUserStatus(input.EmailAddress, true);
        }

        [HttpPost]
        [NccAuth]
        public async Task UpdateOnboardStatus(InputOnboardFromHRMDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var requestCV = await WorkScope.GetAll<RequestCV>()
                .Where(x => x.CV.Email == input.Email)
                .OrderByDescending(x => x.CreationTime)
                .FirstOrDefaultAsync();

                if (requestCV == default)
                {
                    return;
                }
                await _candidateManager.CreateRequestCVStatusChangeHistory(new StatusChangeRequestCVDto
                {
                    Id = requestCV.Id,
                    FromStatus = requestCV.Status,
                    ToStatus = Constants.Enum.RequestCVStatus.Onboarded,
                });
                requestCV.Status = Constants.Enum.RequestCVStatus.Onboarded;
                await _candidateManager.CreateRequestCVHistory(new HistoryRequestCVDto
                {
                    Id = requestCV.Id,
                    Status = requestCV.Status,
                    OnboardDate = requestCV.OnboardDate,
                });

                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }


    }
}
