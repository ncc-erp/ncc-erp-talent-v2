using Abp.Domain.Services;
using System;

namespace TalentV2.DomainServices.BackgroundWorkers
{
    public interface IBackgroundWorkerManager : IDomainService
    {
        event EventHandler<BackgroundWorkerUpdatedEventArgs> WorkerUpdated;
        Entities.BackgroundWorker Create(Entities.BackgroundWorker worker);
        Entities.BackgroundWorker Update(Entities.BackgroundWorker worker);
        void PauseBackgroundWorker(string workerName);
        Entities.BackgroundWorker GetBackgroundWorkerByName(string workerName);
    }
}
