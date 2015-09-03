using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Client
{
    public abstract class LoadTestBase : ILoadTestScenario
    {
        public virtual void ScenarioSetup(ITestContext testContext)
        {
        }

        public virtual void IterationSetup(ITestContext testContext)
        {
        }

        public abstract void ExecuteScenario(ITestContext testContext);

        public virtual void IterationTearDown(ITestContext testContext)
        {
        }

        public virtual void ScenarioTearDown(ITestContext testContext)
        {
        }
    }
}