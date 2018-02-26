using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Pool;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer;

namespace Viki.LoadRunner.Engine.Validators
{

    public static class ScenarioValidatorExtensions
    {
        // TODO: Redesign validator so it uses actual handlers.

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
        public static IterationResult Validate(this IScenario scenario, int threadId = 0, int threadIterationId = 0, int globalIterationId = 0)
        {
            ExecutionTimer timer = CreateTimer();
            IterationContext iteration = CreateIterationContext(threadId, timer);

            ScenarioSetup(scenario, iteration);

            IterationSetup(scenario, threadIterationId, globalIterationId, iteration);

            IterationExecute(scenario, timer, iteration);

            IterationTearDown(scenario, iteration);

            IterationResult result = CollectResults(iteration);

            ScenarioTearDown(scenario, iteration);

            return result;
        }

        private static IterationContext CreateIterationContext(int threadId, ExecutionTimer timer)
        {
            return new IterationContext(threadId, timer);
        }

        private static ExecutionTimer CreateTimer()
        {
            return new ExecutionTimer();
        }

        private static void ScenarioSetup(IScenario scenario, IterationContext iteration)
        {
            iteration.Reset(-1, -1);
            scenario.ScenarioSetup(iteration);
        }

        private static void IterationSetup(IScenario scenario, int threadIterationId, int globalIterationId, IterationContext iteration)
        {
            iteration.Reset(threadIterationId, globalIterationId);
            iteration.Checkpoint(Checkpoint.Names.Setup);

            scenario.IterationSetup(iteration);
        }

        private static void IterationExecute(IScenario scenario, ExecutionTimer timer, IterationContext iteration)
        {
            iteration.Checkpoint(Checkpoint.Names.IterationStart);

            timer.Start();
            iteration.Start();
            scenario.ExecuteScenario(iteration);
            iteration.Stop();
            timer.Stop();

            iteration.Checkpoint(Checkpoint.Names.IterationEnd);
        }

        private static void IterationTearDown(IScenario scenario, IterationContext iteration)
        {
            iteration.Checkpoint(Checkpoint.Names.TearDown);
            scenario.IterationTearDown(iteration);
        }
        private static IterationResult CollectResults(IterationContext iteration)
        {
            return new IterationResult(iteration, new WorkerThreadStats());
        }

        private static void ScenarioTearDown(IScenario scenario, IterationContext iteration)
        {
            iteration.Reset(-1, -1);
            scenario.ScenarioTearDown(iteration);
        }
    }
}