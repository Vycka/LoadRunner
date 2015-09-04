using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine
{
    public interface ILoadTestScenario
    {
        void ScenarioSetup(ITestContext testContext);

        void IterationSetup(ITestContext testContext);

        /// <summary>
        /// Test scenario for single thread
        /// </summary>
        /// <param name="testContext"></param>
        void ExecuteScenario(ITestContext testContext);


        void IterationTearDown(ITestContext testContext);

        void ScenarioTearDown(ITestContext testContext);
    }
}