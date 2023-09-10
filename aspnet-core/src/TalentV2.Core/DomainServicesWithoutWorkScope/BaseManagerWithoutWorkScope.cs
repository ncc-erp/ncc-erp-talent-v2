using Abp.Domain.Services;
using Abp.Runtime.Session;

namespace TalentV2.DomainServicesWithoutWorkScope
{
    public class BaseManagerWithoutWorkScope : DomainService
    {
        protected IAbpSession abpSession;

        public BaseManagerWithoutWorkScope()
        {
            abpSession = NullAbpSession.Instance;
        }
    }
}
