using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Scenario
{
    public abstract class LoadTestBase : ILoadTestScenario
    {
        public virtual void ScenarioSetup(IIterationContext context)
        {
        }

        public virtual void IterationSetup(IIterationContext context)
        {
        }

        public abstract void ExecuteScenario(IIterationContext context);

        public virtual void IterationTearDown(IIterationContext context)
        {
        }

        public virtual void ScenarioTearDown(IIterationContext context)
        {
        }
    }
}