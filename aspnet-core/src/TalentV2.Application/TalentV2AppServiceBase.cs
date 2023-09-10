using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using TalentV2.Authorization.Users;
using TalentV2.MultiTenancy;
using TalentV2.NccCore;
using Abp.Dependency;
using TalentV2.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Linq;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using TalentV2.Utils;
using TalentV2.Configuration;
using TalentV2.DomainServices.Dto;
using Microsoft.AspNetCore.Http;

namespace TalentV2
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class TalentV2AppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }
        public UserManager UserManager { get; set; }
        protected IWorkScope WorkScope { get; set; }
        protected TalentV2DbContext dbContext { get; set; }
        
        protected TalentV2AppServiceBase()
        {
            LocalizationSourceName = TalentV2Consts.LocalizationSourceName;
            WorkScope = IocManager.Instance.Resolve<IWorkScope>();
            dbContext = IocManager.Instance.Resolve<TalentV2DbContext>();
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
        protected virtual async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            UpdateTenantIdEntities(entities);
            await dbContext.AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();
        }
        private void UpdateTenantIdEntities<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
            {
                UpdateTenantId(entity);
                UpdateLastModified(entity);
                Type entityType = typeof(T);
                PropertyInfo[] genericProperties = entityType
                    .GetProperties()
                    .Where(x => x.PropertyType.IsGenericType)
                    .ToArray();
                if (genericProperties.Length > 0)
                {
                    foreach (PropertyInfo propertyInfo in genericProperties)
                    {
                        var propertyValue = propertyInfo.GetValue(entity, null) as IEnumerable;
                        if (propertyValue != null)
                        {
                            List<dynamic> listData = propertyValue.OfType<dynamic>().ToList();
                            UpdateTenantIdEntities(listData);
                        }
                    }
                }
            }
            return;
        }
        private void UpdateTenantId<T>(T entity) where T : class
        {
            var tenantEntity = entity as IMayHaveTenant;
            if (tenantEntity != null)
                tenantEntity.TenantId = CurrentUnitOfWork.GetTenantId();
        }
        private void UpdateLastModified<T>(T entity) where T : class
        {
            var tenantEntity = entity as AuditedEntity<long>;
            if(tenantEntity != null)
            {
                tenantEntity.LastModifierUserId = AbpSession.UserId;
                tenantEntity.LastModificationTime = DateTimeUtils.GetNow();
            }
        }
        protected virtual string GetSpacingString(int charecter)
        {
            string result = new String(' ', charecter);
            return result;
        }
        protected string GetAssigneeName(long id)
        {
            string userName;
            userName = WorkScope.GetAll<User>().Where(a => a.Id == id).Select(s => $"{s.Surname} {s.Name}").FirstOrDefault();
            return userName;
        }
    }
}
