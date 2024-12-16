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

        public NCCBackgroundWorkerBase(AbpTimer timer) : base(timer)
        {
            UnitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
            WorkerManager = IocManager.Instance.Resolve<DomainServices.BackgroundWorkers.IBackgroundWorkerManager>();
            WorkerManager.WorkerUpdated += BackgroundWorkerManager_WorkerUpdated;
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
            if (Worker != null)
            {
                Timer.Period = Worker.Period;
            }
            Worker ??= WorkerManager.Create(new Entities.BackgroundWorker
            {
                Name = WorkerName,
                IsPaused = false,
                Period = Timer.Period,
                State = BackgroundWorkerState.WaitToRun
            });
            uow.Complete();
        }

        private void BackgroundWorkerManager_WorkerUpdated(object sender, DomainServices.BackgroundWorkers.BackgroundWorkerUpdatedEventArgs e)
        {
            Worker = e.BackgroundWorker;
            Timer.Period = Worker.Period;
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
            if (!DoWorkStartCondition())
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
