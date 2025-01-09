using Abp.Dependency;
using Abp.Threading.Timers;
using System;

namespace TalentV2.BackgroundWorker
{
    public class TestNCCWorker : NCCBackgroundWorkerBase<TestNCCWorker>, ISingletonDependency
    {
        public TestNCCWorker(AbpTimer timer) : base(timer)
        {
            Timer.RunOnStart = true;
            InitialPeriod = 2000;
        }

        protected override void DoWorkMainLogic()
        {
        }
    }
}
