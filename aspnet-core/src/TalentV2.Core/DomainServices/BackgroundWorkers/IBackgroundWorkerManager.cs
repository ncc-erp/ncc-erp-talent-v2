using Abp.Domain.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.DomainServices.BackgroundWorkers.Dtos;

namespace TalentV2.DomainServices.BackgroundWorkers
{
    public interface IBackgroundWorkerManager : IDomainService
    {
        public IQueryable<BackgroundWorkerDto> IQGetAllBackgroundWorker();
        public Entities.BackgroundWorker Create(Entities.BackgroundWorker worker);
        public Entities.BackgroundWorker Update(Entities.BackgroundWorker worker);
        public void PauseBackgroundWorker(string workerName);
        public Entities.BackgroundWorker GetBackgroundWorkerByName(string workerName);

        public BackgroundWorkerDto UpdateActivedWorker(BackgroundWorkerDto worker);
    }
}
