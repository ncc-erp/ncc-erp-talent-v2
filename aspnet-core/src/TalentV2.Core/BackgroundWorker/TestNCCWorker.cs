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
        }

        protected override void DoWorkMainLogic()
        {
        }
    }
}
