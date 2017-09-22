using System;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Scenario;
using Viki.LoadRunner.Engine.Executor.Threads.Scheduler;
using Viki.LoadRunner.Engine.Executor.Threads.Stats;
using Viki.LoadRunner.Engine.Executor.Threads.Strategy;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Framework;
using Viki.LoadRunner.Engine.Strategies;

namespace Viki.LoadRunner.Engine.Executor.Threads.Factory
{
    public class ScenarioThreadFactory : IThreadFactory
    {
        private readonly Type _scenarioType;
        private readonly ITimer _timer;
        private readonly ISpeedStrategy _speedStrategy;
        private readonly IThreadPoolCounter _counter;
        private readonly IResultsAggregator _aggregator;

        private readonly IterationContextFactory _iterationContextFactory;
        private readonly IUniqueIdFactory<int> _globalIdFactory;

        public ScenarioThreadFactory(Type scenarioType, ITimer timer, ISpeedStrategy speedStrategy, IThreadPoolCounter counter, object initialUserData, IResultsAggregator aggregator, IUniqueIdFactory<int> globalIdFactory)
        {
            if (scenarioType == null)
                throw new ArgumentNullException(nameof(scenarioType));
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (speedStrategy == null)
                throw new ArgumentNullException(nameof(speedStrategy));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));
            if (globalIdFactory == null)
                throw new ArgumentNullException(nameof(globalIdFactory));

            _scenarioType = scenarioType;
            _timer = timer;
            _speedStrategy = speedStrategy;
            _counter = counter;
            _aggregator = aggregator;

            _iterationContextFactory = new IterationContextFactory(_timer, initialUserData);
            _globalIdFactory = globalIdFactory;
        }

        public IWorkerThread Create()
        {
            ILoadTestScenario scenarioInstance = (ILoadTestScenario)Activator.CreateInstance(_scenarioType);
            IIterationContextControl iterationContext = _iterationContextFactory.Create();

            ScenarioHandlerEx scenarioHandler = new ScenarioHandlerEx(_globalIdFactory, scenarioInstance, iterationContext);

            IIterationState iterationState = new IterationState(_timer, iterationContext, _counter);

            SpeedStrategyHandler strategyHandler = new SpeedStrategyHandler(_speedStrategy, iterationState);
            SchedulerEx scheduler = new SchedulerEx(strategyHandler, _counter, _timer);

            // TestContext still needs to have a split here, otherwise DataCollector looks like crap
            // smth like IIterationResult to keep modifieble state
            // Control part controls one part, and Context part controls other.
            // Or still monolith context with redesigned interfaces on it from scratch
            // SHould not compile atm

            DataCollector collector = new DataCollector(iterationContext, _aggregator, _counter);

            //  

            ScenarioWorkerTask scenarioWorkerTask = new ScenarioWorkerTask(scheduler, scenarioHandler, collector);

            IWorkerThread thread = new WorkerThreadEx(scenarioWorkerTask);

            return thread;
        }
    }

    public interface IThreadFactory
    {
        IWorkerThread Create();
    }
}