using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine
{
    public interface ITestScenario
    {
        void ScenarioSetup(ITestContext testContext);
        void ScenarioTearDown(ITestContext testContext);

        void IterationSetup(ITestContext testContext);

        void IterationTearDown(ITestContext testContext);

        /// <summary>
        /// Test scenario for single thread
        /// </summary>
        /// <param name="testContext"></param>
        void ExecuteScenario(ITestContext testContext);
    }
}