using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using NccCore.Extension;
using System;
using System.Linq;
using TalentV2.MultiTenancy;
using TalentV2.Constants.Enum;
using Abp.UI;
using TalentV2.DomainServices.BackgroundWorkers.Dtos;


namespace TalentV2.DomainServices.BackgroundWorkers
{
    public class BackgroundWorkerManager : BaseManager, IBackgroundWorkerManager
    {
        public IUnitOfWorkManager _UnitOfWorkManager { get; private set; }
        private readonly IRepository<Tenant> _TenantRepository;

        public BackgroundWorkerManager(IUnitOfWorkManager unitOfWorkManager,
            IRepository<Tenant> tenantRepository)
        {
            _UnitOfWorkManager = unitOfWorkManager;
            _TenantRepository = tenantRepository;
        }

        public Entities.BackgroundWorker Create(Entities.BackgroundWorker worker)
        {
            var result = WorkScope.GetRepo<Entities.BackgroundWorker>()
                .Insert(worker);
            return result;
        }

        public Entities.BackgroundWorker Update(Entities.BackgroundWorker worker)
        {
            Entities.BackgroundWorker updatedWorker;

            using (var uow = _UnitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                updatedWorker = WorkScope.GetRepo<Entities.BackgroundWorker>()
                        .Update(worker);
                uow.Complete();
            }

            return updatedWorker;
        }

        public void PauseBackgroundWorker(string workerName)
        {
            var worker = GetBackgroundWorkerByName(workerName);
            if (worker.State == BackgroundWorkerState.Running)
            {
                return;
            }

            worker.IsPaused = true;
            Update(worker);
        }

        public BackgroundWorkerDto UpdateActivedWorker(BackgroundWorkerDto worker)
        {
            Entities.BackgroundWorker runningWorker = GetBackgroundWorkerByName(worker.Name);
            if (runningWorker == null)
            {
                throw new UserFriendlyException("BackgroundWorker not found");
            }
            
            if (worker.TenantId != null &&
                !_TenantRepository.GetAll().Any(x => x.Id == worker.TenantId))
            {
                throw new UserFriendlyException("TenantId does not exist");
            }

            if (worker.IsPaused == true && worker.State == BackgroundWorkerState.Running)
            {
                throw new UserFriendlyException("BackgroundWorker can not pause while it is running!");
            }

            runningWorker.Period = worker.Period;
            runningWorker.TenantId = worker.TenantId;
            runningWorker.IsPaused = worker.IsPaused;

            Type type = Type.GetType(runningWorker.Name);
            var backgroundWorkerInstance = IocManager.Instance.Resolve(type);

            var method = type.GetMethod("UpdateBackgroundWorker");
            if (method != null)
            {
                method.Invoke(backgroundWorkerInstance, new object[] { runningWorker });
            }

            BackgroundWorkerDto result = ObjectMapper.Map<BackgroundWorkerDto>(runningWorker);
            return result;
        }

        public Entities.BackgroundWorker GetBackgroundWorkerByName(string workerName)
        {
            return WorkScope.GetAll<Entities.BackgroundWorker, long>()
                .Where(x => x.Name.Equals(workerName))
                .FirstOrDefault();
        }
    }

}
