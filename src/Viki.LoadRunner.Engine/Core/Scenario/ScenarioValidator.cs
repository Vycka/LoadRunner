using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Pool;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer;

namespace Viki.LoadRunner.Engine.Core.Scenario
{
    /// <summary>
    /// ILoadTestScenario validator, to ease development and debugging.
    /// </summary>
    public static class ScenarioValidator
    {
        /// <summary>
        /// Validates ILoadTest scenario correctness by executing single test iteration
        /// from ScenarioSetup to ScenarioTearDown on the same thread.
        /// Exceptions are not handled on purpose to ease problem identification while developing.
        /// </summary>
        /// <param name="scenario">ILoadTestScenario object</param>
        /// <param name="threadId">TheardId to set in TestContext</param>
        /// <param name="threadIterationId">ThreadIterationId to set in TestContext</param>
        /// <param name="globalIterationId">GlobalIterationId to set in TestContext</param>
        /// <returns>Raw result from single iteration</returns>
        public static IterationResult Validate(IScenario scenario, int threadId = 0, int threadIterationId = 0, int globalIterationId = 0)
        {
            ExecutionTimer timer = new ExecutionTimer();

            Iteration iteration =  new Iteration(threadId, timer);
            iteration.Reset(-1, -1);
            scenario.ScenarioSetup(iteration);

            iteration.Reset(threadIterationId, globalIterationId);
            iteration.Checkpoint(Checkpoint.Names.Setup);

            scenario.IterationSetup(iteration);

            iteration.Checkpoint(Checkpoint.Names.IterationStart);

            timer.Start();
            iteration.Start();
            scenario.ExecuteScenario(iteration);
            iteration.Stop();
            timer.Stop();

            iteration.Checkpoint(Checkpoint.Names.IterationEnd);

            iteration.Checkpoint(Checkpoint.Names.TearDown);
            scenario.IterationTearDown(iteration);

            IterationResult result = new IterationResult(iteration, new WorkerThreadStats());

            iteration.Reset(-1, -1);
            scenario.ScenarioTearDown(iteration);

            return result;
        }
    }
}