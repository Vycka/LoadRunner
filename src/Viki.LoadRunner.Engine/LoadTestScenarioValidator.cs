using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;
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
        /// <param name="threadId">TheardId to set in TestContext</param>
        /// <param name="threadIterationId">ThreadIterationId to set in TestContext</param>
        /// <param name="globalIterationId">GlobalIterationId to set in TestContext</param>
        /// <returns>Raw result from single iteration</returns>
        public static IterationResult Validate(ILoadTestScenario loadTestScenario, int threadId = 0, int threadIterationId = 0, int globalIterationId = 0)
        {
            ExecutionTimer timer = new ExecutionTimer();

            TestContext testContext =  new TestContext(threadId, timer);
            testContext.Reset(-1, -1);
            loadTestScenario.ScenarioSetup(testContext);

            testContext.Reset(threadIterationId, globalIterationId);
            testContext.Checkpoint(Checkpoint.IterationSetupCheckpointName);

            loadTestScenario.IterationSetup(testContext);

            testContext.Checkpoint(Checkpoint.IterationStartCheckpointName);

            timer.Start();
            testContext.Start();
            loadTestScenario.ExecuteScenario(testContext);
            testContext.Stop();
            timer.Stop();

            testContext.Checkpoint(Checkpoint.IterationEndCheckpointName);

            testContext.Checkpoint(Checkpoint.IterationTearDownCheckpointName);
            loadTestScenario.IterationTearDown(testContext);

            IterationResult result = new IterationResult(testContext);

            testContext.Reset(-1, -1);
            loadTestScenario.ScenarioTearDown(testContext);

            return result;
        }
    }
}