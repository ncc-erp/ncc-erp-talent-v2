using TalentV2.Authorization.Users;
using TalentV2.DomainServices.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Roles;

namespace TalentV2.DomainServices.Users
{
    public class UserManager : BaseManager, IUserManager
    {
        private readonly RoleManager _roleManager;
        public UserManager(RoleManager roleManager) 
        {
            _roleManager = roleManager;
        }
        public IQueryable<AllUserPagingDto> IQGetAllUserPaging()
        {
            var qallUserPaging = from u in WorkScope.GetAll<User>()
                                 select new AllUserPagingDto
                                 {
                                     Id = u.Id,
                                     EmailAddress = u.EmailAddress,
                                     Name = u.Name,
                                     IsActive = u.IsActive,
                                     Surname = u.Surname,
                                     UserName = u.UserName,
                                     BranchId = u.BranchId,
                                     BranchName = u.Branch.Name, 
                                     RoleNames =(from ur in u.Roles
                                                join r in _roleManager.Roles on ur.RoleId equals r.Id 
                                                where ur.UserId == u.Id
                                                select r.Name).ToArray(),
                                     CreationTime = u.CreationTime,
                                     LastModifiedTime = u.LastModificationTime

                                 };
            return qallUserPaging;
        }
    }
}
