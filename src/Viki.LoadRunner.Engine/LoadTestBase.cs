using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine
{
    public abstract class LoadTestBase : ILoadTestScenario
    {
        public virtual void ScenarioSetup(ITestContext context)
        {
        }

        public virtual void IterationSetup(ITestContext context)
        {
        }

        public abstract void ExecuteScenario(ITestContext context);

        public virtual void IterationTearDown(ITestContext context)
        {
        }

        public virtual void ScenarioTearDown(ITestContext context)
        {
        }
    }
}