using Abp.Extensions;
using System;
using System.Linq;

namespace TalentV2.DomainServices.BackgroundWorkers
{
    public class BackgroundWorkerManager : BaseManager, IBackgroundWorkerManager
    {
        public event EventHandler<BackgroundWorkerUpdatedEventArgs> WorkerUpdated;

        public Entities.BackgroundWorker Create(Entities.BackgroundWorker worker)
        {
            return WorkScope.GetRepo<Entities.BackgroundWorker>()
                .Insert(worker);
        }

        public Entities.BackgroundWorker Update(Entities.BackgroundWorker worker)
        {
            var workerUpdated = WorkScope.GetRepo<Entities.BackgroundWorker>()
                .Update(worker);

            WorkerUpdated.InvokeSafely(this, new BackgroundWorkerUpdatedEventArgs(worker));
            return workerUpdated;
        }

        public void PauseBackgroundWorker(string workerName)
        {
            // Need to check BackgroundWorkerState, user can not pause BackgroundWorker if BackgroundWorkerState == Running
            var worker = GetBackgroundWorkerByName(workerName);
            worker.IsPaused = true;
            Update(worker);
        }

        public Entities.BackgroundWorker GetBackgroundWorkerByName(string workerName)
        {
            return WorkScope.GetAll<Entities.BackgroundWorker, long>()
                .Where(x => x.Name.Equals(workerName))
                .FirstOrDefault();
        }
    }

    public class BackgroundWorkerUpdatedEventArgs : EventArgs
    {
        public Entities.BackgroundWorker BackgroundWorker { get; set; }

        public BackgroundWorkerUpdatedEventArgs(Entities.BackgroundWorker backgroundWorker)
        {
            BackgroundWorker = backgroundWorker;
        }
    }
}
