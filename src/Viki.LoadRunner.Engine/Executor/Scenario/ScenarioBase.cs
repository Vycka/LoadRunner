using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Scenario
{
    public abstract class ScenarioBase : IScenario
    {
        public virtual void ScenarioSetup(IIteration context)
        {
        }

        public virtual void IterationSetup(IIteration context)
        {
        }

        public abstract void ExecuteScenario(IIteration context);

        public virtual void IterationTearDown(IIteration context)
        {
        }

        public virtual void ScenarioTearDown(IIteration context)
        {
        }
    }
}