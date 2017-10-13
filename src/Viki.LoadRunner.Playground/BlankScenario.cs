using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Executor.Context.Interfaces;

namespace Viki.LoadRunner.Playground
{
    public class BlankScenario : ILoadTestScenario
    {
        public void ScenarioSetup(IIterationContext context)
        {
            
        }

        public void IterationSetup(IIterationContext context)
        {

        }

        public void ExecuteScenario(IIterationContext context)
        {
            //Thread.Sleep(200);
        }

        public void IterationTearDown(IIterationContext context)
        {

        }

        public void ScenarioTearDown(IIterationContext context)
        {

        }
    }
}