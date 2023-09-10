using Abp.Dependency;
using Abp.Domain.Services;
using TalentV2.NccCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Session;
using Castle.Core.Logging;

namespace TalentV2.DomainServices
{
    public abstract class BaseManager : DomainService
    {
        protected IWorkScope WorkScope;
        protected IAbpSession AbpSession;
        protected ILogger Logger;
        protected BaseManager()
        {
            WorkScope = IocManager.Instance.Resolve<IWorkScope>();
            AbpSession = IocManager.Instance.Resolve<IAbpSession>();
            Logger = NullLogger.Instance;
        }
    }
}
