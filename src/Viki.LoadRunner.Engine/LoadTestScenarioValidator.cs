using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Timer;

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
        public static TestContextResult Validate(ILoadTestScenario loadTestScenario, int threadId = 0, int threadIterationId = 0, int globalIterationId = 0)
        {
            TestContext testContext =  new TestContext(threadId, new ExecutionTimer());
            testContext.Reset(-1, -1);
            loadTestScenario.ScenarioSetup(testContext);

            testContext.Reset(threadIterationId, globalIterationId);
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

            testContext.Reset(-1, -1);
            loadTestScenario.ScenarioTearDown(testContext);

            return result;
        }
    }
}