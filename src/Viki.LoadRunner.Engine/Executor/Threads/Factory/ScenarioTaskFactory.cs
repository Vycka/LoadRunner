using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Context.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Counters.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Scenario;
using Viki.LoadRunner.Engine.Executor.Threads.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Stats;
using Viki.LoadRunner.Engine.Executor.Threads.Stats.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Strategy;
using Viki.LoadRunner.Engine.Executor.Threads.Workers;
using Viki.LoadRunner.Engine.Executor.Threads.Workers.Interfaces;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Framework;
using Viki.LoadRunner.Engine.Strategies;

namespace Viki.LoadRunner.Engine.Executor.Threads.Factory
{
    // TODO: parts of this factory can easily go to ILoadRunnerSettings 
    // Like final ISpeedStrategy could be built there
    //
    public class ScenarioThreadFactory : IWorkerThreadFactory
    {
        private readonly Type _scenarioType;
        private readonly ITimer _timer;
        private readonly ISpeedStrategy _speedStrategy;
        private readonly IThreadPoolCounter _counter;
        private readonly IResultsAggregator _aggregator;

        private readonly IterationContextFactory _iterationContextFactory;
        private readonly IUniqueIdFactory<int> _globalIdFactory;
        private readonly IErrorHandler _errorHandler;
        private readonly IPrewait _prewait;

        public ScenarioThreadFactory(Type scenarioType, ITimer timer, ISpeedStrategy speedStrategy, IThreadPoolCounter counter, object initialUserData, IResultsAggregator aggregator, IUniqueIdFactory<int> globalIdFactory, IErrorHandler errorHandler)
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
            if (errorHandler == null)
                throw new ArgumentNullException(nameof(errorHandler));

            _scenarioType = scenarioType;
            _timer = timer;
            _speedStrategy = speedStrategy;
            _counter = counter;
            _aggregator = aggregator;
            _errorHandler = errorHandler;

            _iterationContextFactory = new IterationContextFactory(_timer, initialUserData);
            _globalIdFactory = globalIdFactory;
            
            _prewait = new TimerBasedPrewait(timer);
        }

        public IWorkerThread Create()
        {
            // Scenario handler
            ILoadTestScenario scenarioInstance = (ILoadTestScenario)Activator.CreateInstance(_scenarioType);
            IIterationContextControl iterationContext = _iterationContextFactory.Create();

            IScenarioHandler scenarioHandler = new ScenarioHandler(_globalIdFactory, scenarioInstance, iterationContext);


            // Scheduler for ISpeedStrategy
            IIterationState iterationState = new IterationState(_timer, iterationContext, _counter);
            SpeedStrategyHandler strategyHandler = new SpeedStrategyHandler(_speedStrategy, iterationState);

            IScheduler scheduler = new Scheduler.Scheduler(strategyHandler, _counter, _timer);


            // Data collector for results aggregation
            IDataCollector collector = new DataCollector(iterationContext, _aggregator, _counter);


            // Put it all together to create IWork.
            IWork scenarioWork = new ScenarioWork(scheduler, scenarioHandler, collector);



            IWorkerThread thread = new WorkerThread(scenarioWork, _prewait);
            _errorHandler.Register(thread);

            return thread;
        }
    }

    // TODO: This needs to go up to the ILoadRunnerSettings
}