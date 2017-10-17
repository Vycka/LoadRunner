using System;
using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;
using Viki.LoadRunner.Engine.Executor.Factory.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Executor.Worker;
using Viki.LoadRunner.Engine.Executor.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Factory
{
    public class ScenarioThreadFactory : IWorkerThreadFactory
    {
        private readonly IScenarioHandlerFactory _scenarioHandlerFactory;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IDataCollectorFactory _dataCollectorFactory;
        private readonly IIterationContextFactory _iterationContextFactory;
        private readonly IErrorHandler _errorHandler;
        private readonly IPrewait _prewait;

        public ScenarioThreadFactory(IIterationContextFactory iterationContextFactory, IScenarioHandlerFactory scenarioHandlerFactory, ISchedulerFactory schedulerFactory, IDataCollectorFactory dataCollectorFactory, IPrewait prewait, IErrorHandler errorHandler)
        {
            if (iterationContextFactory == null)
                throw new ArgumentNullException(nameof(iterationContextFactory));
            if (scenarioHandlerFactory == null)
                throw new ArgumentNullException(nameof(scenarioHandlerFactory));
            if (schedulerFactory == null)
                throw new ArgumentNullException(nameof(schedulerFactory));
            if (dataCollectorFactory == null)
                throw new ArgumentNullException(nameof(dataCollectorFactory));
            if (prewait == null)
                throw new ArgumentNullException(nameof(prewait));
            if (errorHandler == null)
                throw new ArgumentNullException(nameof(errorHandler));


            _iterationContextFactory = iterationContextFactory;
            _scenarioHandlerFactory = scenarioHandlerFactory;
            _schedulerFactory = schedulerFactory;
            _dataCollectorFactory = dataCollectorFactory;
            _prewait = prewait;
        }

        public IWorkerThread Create()
        {
            IIterationControl iterationContext = _iterationContextFactory.Create();

            // IScenario handler
            IScenarioHandler scenarioHandler = _scenarioHandlerFactory.Create(iterationContext);
            // Scheduler for ISpeedStrategy
            IScheduler scheduler = _schedulerFactory.Create(iterationContext);
            // Data collector for results aggregation
            IDataCollector collector = _dataCollectorFactory.Create(iterationContext);


            // Put it all together to create IWork.
            IWork scenarioWork = new ScenarioWork(scheduler, scenarioHandler, collector);
            IWorkerThread thread = new WorkerThread(scenarioWork, _prewait);
            _errorHandler.Register(thread);

            return thread;
        }
    }
}