using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine
{
    public interface ILoadTestScenario
    {
        void ScenarioSetup(ITestContext context);

        void IterationSetup(ITestContext context);

        /// <summary>
        /// Test scenario for single thread
        /// </summary>
        /// <param name="testContext"></param>
        void ExecuteScenario(ITestContext context);


        void IterationTearDown(ITestContext context);

        void ScenarioTearDown(ITestContext context);
    }
}