using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine
{
    /// <summary>
    /// ILoadTestScenario validator, to ease development and debugging.
    /// </summary>
    public static class LoadTestScenarioValidator
    {
        /// <summary>
        /// Validates ILoadTest scenario correctness by executing single test iteration
        /// from ScenarioSetup to ScenarioTearDown on the same thread.
        /// Exceptions are not handled on purpose to ease problem identification while developing.
        /// </summary>
        /// <param name="loadTestScenario">ILoadTestScenario object</param>
        /// <returns>Raw result from single iteration</returns>
        public static TestContextResult Validate(ILoadTestScenario loadTestScenario)
        {
            TestContext testContext =  new TestContext(0);

            testContext.Reset(0, 0);
            testContext.Checkpoint(Checkpoint.IterationSetupCheckpointName);

            loadTestScenario.IterationSetup(testContext);

            testContext.Checkpoint(Checkpoint.IterationStartCheckpointName);

            testContext.Start();
            loadTestScenario.ExecuteScenario(testContext);
            testContext.Stop();

            testContext.Checkpoint(Checkpoint.IterationEndCheckpointName);

            testContext.Checkpoint(Checkpoint.IterationTearDownCheckpointName);
            loadTestScenario.IterationTearDown(testContext);

            TestContextResult result = new TestContextResult(testContext);

            loadTestScenario.ScenarioTearDown(testContext);

            return result;
        }
    }
}