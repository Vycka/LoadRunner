using System;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class Executor
    {
        private readonly IScheduler _scheduler;

        private readonly ScenarioInstance _instance;
        private readonly IDataCollector _collector;

        private bool _cencellationToken;

        public Executor(IScheduler scheduler, ScenarioInstance instance, IDataCollector collector)
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (collector == null) throw new ArgumentNullException(nameof(collector));

            _scheduler = scheduler;
            _instance = instance;
            _collector = collector;
        }

        public void Setup()
        {
            _instance.Setup();
        }

        public void Execute()
        {
            _instance.PrepareNext();

            _scheduler.Wait();

            if (!_cencellationToken)
            {
                _instance.Execute();

                _collector.Collect(_instance.Context);
            }
        }

        public void Teardown()
        {
            _instance.Teardown();
        }
    }
}