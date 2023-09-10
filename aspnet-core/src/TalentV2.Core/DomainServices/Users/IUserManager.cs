using Abp.Dependency;
using TalentV2.DomainServices.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace TalentV2.DomainServices.Users
{
    public interface IUserManager : IDomainService
    {
        IQueryable<AllUserPagingDto> IQGetAllUserPaging();
    }
}
