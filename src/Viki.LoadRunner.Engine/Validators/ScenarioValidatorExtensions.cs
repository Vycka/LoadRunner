using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Counter;
using Viki.LoadRunner.Engine.Core.Generator;
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

            ExecutionTimer timer = new ExecutionTimer();
            ThreadSafeCounter errorsCounter = new ThreadSafeCounter();
            GlobalCounters globalCounters = new GlobalCounters(errorsCounter, new MockedIdGenerator(globalIterationId));
            IIterationControl context = new IterationContext(threadId, timer);

            ScenarioHandler handler = new ScenarioHandler(globalCounters, new MockedIdGenerator(threadIterationId), scenario, context);

            timer.Start();

            handler.Init();
            handler.PrepareNext();
            handler.Execute();

            IterationResult result = new IterationResult(context, new WorkerThreadStats());

            handler.Cleanup();

            timer.Stop();

            return result;
        }
    }
}