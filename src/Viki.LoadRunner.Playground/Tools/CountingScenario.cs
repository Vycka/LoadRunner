using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Playground.Tools
{
    public class CountingScenario : IScenario
    {
        public long Count = 0;
        public int ThreadId;

        public void ScenarioSetup(IIteration context)
        {
            ThreadId = context.ThreadId;
        }

        public void IterationSetup(IIteration context)
        {

        }

        public void ExecuteScenario(IIteration context)
        {
            Count = Count + 1;
        }

        public void IterationTearDown(IIteration context)
        {

        }

        public void ScenarioTearDown(IIteration context)
        {

        }
    }
}