using System;
using Viki.LoadRunner.Engine.Executor.Threads.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Stats.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Threads.Scenario
{
    public class ScenarioWork : IWork
    {
        private readonly IScheduler _scheduler;
        private readonly IScenarioHandler _handler;

        private readonly IDataCollector _collector;

        private bool _stopping = false;

        public ScenarioWork(IScheduler scheduler, IScenarioHandler handler, IDataCollector collector)
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (collector == null)
                throw new ArgumentNullException(nameof(collector));

            _scheduler = scheduler;
            _handler = handler;
            _collector = collector;
        }

        public void Init()
        {
            _handler.Init();
        }

        public void Execute()
        {
            _handler.PrepareNext();

            _scheduler.WaitNext(ref _stopping);

            if (!_stopping)
            {
                _handler.Execute();

                _collector.Collect();
            }
        }

        public void Cleanup()
        {
            _handler.Cleanup();
        }

        public void Stop()
        {
            _stopping = true;
        }
    }
}