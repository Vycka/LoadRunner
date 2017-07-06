using System.Threading;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Playground
{
    public class BlankScenario : ILoadTestScenario
    {
        public void ScenarioSetup(ITestContext context)
        {
            
        }

        public void IterationSetup(ITestContext context)
        {

        }

        public void ExecuteScenario(ITestContext context)
        {
            Thread.Sleep(200);
        }

        public void IterationTearDown(ITestContext context)
        {

        }

        public void ScenarioTearDown(ITestContext context)
        {

        }
    }
}