using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using TalentV2.Constants.Enum;

namespace TalentV2.BackgroundWorker
{
    public abstract class NCCBackgroundWorkerBase<T> : PeriodicBackgroundWorkerBase where T : class
    {
        protected virtual string WorkerName { get; set; }

        public DomainServices.BackgroundWorkers.IBackgroundWorkerManager WorkerManager { get; private set; }
        public IUnitOfWorkManager UnitOfWorkManager { get; private set; }

        public Entities.BackgroundWorker Worker { get; private set; }

        private const int InitialPeriod = 1000 * 5 * 1 ;  //5 seconds

        public NCCBackgroundWorkerBase(AbpTimer timer) : base(timer)
        {
            UnitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
            WorkerManager = IocManager.Instance.Resolve<DomainServices.BackgroundWorkers.IBackgroundWorkerManager>();
            WorkerName = typeof(T).FullName;
            InitializeBackgroundWorker();
        }

        public override void Start()
        {
            base.Start();
        }

        private void InitializeBackgroundWorker()
        {
            using var uow = UnitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew);
            Worker = WorkerManager.GetBackgroundWorkerByName(WorkerName);

            Worker ??= WorkerManager.Create(new Entities.BackgroundWorker
            {
                Name = WorkerName,
                IsPaused = false,
                Period = InitialPeriod,
                State = BackgroundWorkerState.WaitToRun
            });

            Timer.Period = Worker.Period;
            uow.Complete();
        }

        public void UpdateBackgroundWorker(Entities.BackgroundWorker worker)
        {
            Timer.Period = worker.Period;

            Worker.Period = worker.Period;
            Worker.IsPaused = worker.IsPaused;
            Worker.TenantId = worker.TenantId;

            WorkerManager.Update(Worker);
        }

        private void UpdateBackgroundWorkerState(BackgroundWorkerState state)
        {
            Worker.State = state;
            WorkerManager.Update(Worker);
        }

        protected abstract void DoWorkMainLogic();

        protected virtual bool DoWorkStartCondition()
        {
            return Worker.IsPaused;
        }

        protected override void DoWork()
        {
            if (DoWorkStartCondition())
            {
                UpdateBackgroundWorkerState(BackgroundWorkerState.Paused);
                return;
            }

            UpdateBackgroundWorkerState(BackgroundWorkerState.Running);
            DoWorkMainLogic();
            UpdateBackgroundWorkerState(BackgroundWorkerState.WaitToRun);
        }
    }
}
