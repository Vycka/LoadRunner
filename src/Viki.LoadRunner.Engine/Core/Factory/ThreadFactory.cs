using System;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
{
    public class ThreadFactory : IScenarioThreadFactory
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IPrewait _prewait;

        public ThreadFactory(IPrewait prewait, IErrorHandler errorHandler)
        {
            if (prewait == null)
                throw new ArgumentNullException(nameof(prewait));
            if (errorHandler == null)
                throw new ArgumentNullException(nameof(errorHandler));

            _prewait = prewait;
            _errorHandler = errorHandler;
        }

        public IThread Create(IScheduler scheduler, IScenarioHandler handler, IDataCollector collector)
        {
            IWork scenarioWork = new ScenarioWork(scheduler, handler, collector);
            IThread thread = new WorkerThread(scenarioWork, _prewait);
            _errorHandler.Register(thread);

            return thread;
        }
    }
}