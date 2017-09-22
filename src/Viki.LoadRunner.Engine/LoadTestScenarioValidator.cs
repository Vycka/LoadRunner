using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;
using Viki.LoadRunner.Engine.Executor.Threads;
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

            IterationContext iterationContext =  new IterationContext(threadId, timer);
            iterationContext.Reset(-1, -1);
            loadTestScenario.ScenarioSetup(iterationContext);

            iterationContext.Reset(threadIterationId, globalIterationId);
            iterationContext.Checkpoint(Checkpoint.Names.Setup);

            loadTestScenario.IterationSetup(iterationContext);

            iterationContext.Checkpoint(Checkpoint.Names.IterationStart);

            timer.Start();
            iterationContext.Start();
            loadTestScenario.ExecuteScenario(iterationContext);
            iterationContext.Stop();
            timer.Stop();

            iterationContext.Checkpoint(Checkpoint.Names.IterationEnd);

            iterationContext.Checkpoint(Checkpoint.Names.TearDown);
            loadTestScenario.IterationTearDown(iterationContext);

            IterationResult result = new IterationResult(iterationContext, new WorkerThreadStats());

            iterationContext.Reset(-1, -1);
            loadTestScenario.ScenarioTearDown(iterationContext);

            return result;
        }
    }
}