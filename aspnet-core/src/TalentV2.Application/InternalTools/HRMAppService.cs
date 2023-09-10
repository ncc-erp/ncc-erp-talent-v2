using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;
using TalentV2.InternalTools.Dtos;
using TalentV2.Ncc;
using TalentV2.NccCore;
using TalentV2.Utils;

namespace TalentV2.InternalTools
{
    public class HRMAppService : TalentV2AppServiceBase
    {
        private readonly UserManager _userManager;
        private readonly IWorkScope _ws;
        public HRMAppService(UserManager userManager, IWorkScope ws)
        {
            _userManager = userManager;
            _ws = ws;
        }
        [HttpPost]
        [NccAuth]
        public async Task CreateUserFromHRM(CreateUserFromHRMDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var user = ObjectMapper.Map<User>(input);

                user.Password = PasswordUtils.GeneratePassword(8,true);
                user.TenantId = AbpSession.TenantId;
                user.IsEmailConfirmed = true;

                await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

                CheckErrors(await _userManager.CreateAsync(user, input.Password));

                CheckErrors(await _userManager.SetRolesAsync(user, new string[] { StaticRoleNames.Tenants.BasicUser }));

                CurrentUnitOfWork.SaveChanges();
            }
        }
        [HttpPost]
        [NccAuth]
        public async Task UpdateUserFromHRM(UpdateUserFromHRMDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var user = await _ws.GetAll<User>().Where(s => s.EmailAddress == input.EmailAddress).FirstOrDefaultAsync();

                ObjectMapper.Map(input, user);

                CheckErrors(await _userManager.UpdateAsync(user));

                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
        [HttpPost]
        [NccAuth]
        public async Task ChangeUserStatusFromHRM(ChangeTalentUserStatusDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var user = await _ws.GetAll<User>().Where(s => s.EmailAddress == input.EmailAddress).FirstOrDefaultAsync();

                user.IsActive = input.IsActive;

                CheckErrors(await _userManager.UpdateAsync(user));

                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
    }
}
