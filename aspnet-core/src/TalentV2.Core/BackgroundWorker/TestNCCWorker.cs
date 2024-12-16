using Abp.Dependency;
using Abp.Threading.Timers;

namespace TalentV2.BackgroundWorker
{
    public class TestNCCWorker : NCCBackgroundWorkerBase<TestNCCWorker>, ISingletonDependency
    {
        public TestNCCWorker(AbpTimer timer) : base(timer)
        {
            Timer.RunOnStart = true;
            Timer.Period = 10000;
        }

        protected override void DoWorkMainLogic()
        {
        }
    }
}
