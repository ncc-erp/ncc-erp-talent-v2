using Abp.Domain.Entities;
using NccCore.Anotations;
using TalentV2.Constants.Enum;

namespace TalentV2.Entities
{
    public class BackgroundWorker : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public BackgroundWorkerState State { get; set; }
        public int Period { get; set; }
        public bool IsPaused { get; set; }
    }
}
