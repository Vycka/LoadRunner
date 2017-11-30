using Viki.LoadRunner.Engine.Core.Collector;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Data;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;

namespace Viki.LoadRunner.Engine.Validators
{
    public static class ScenarioValidatorExtensions
    {
        private static readonly ScenarioValidator Validator = new ScenarioValidator();

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
            return Validator.Validate(scenario, threadId, threadIterationId, globalIterationId);
        }

        public static IterationResult Validate<TData>(this IReplayScenario<TData> scenario, DataItem dataItem)
        {
            return ReplayScenarioValidator<TData>.Validate(scenario, dataItem);
        }
    }
}