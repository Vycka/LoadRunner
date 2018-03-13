using System;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Data.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scheduler;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Factory
{
    public class ReplaySchedulerFactory : IReplaySchedulerFactory
    {
        private readonly ITimer _timer;
        private readonly IReplayDataReader _dataReader;
        private readonly IThreadPoolCounter _counter;
        private readonly double _speedMultiplier;


        public ReplaySchedulerFactory(ITimer timer, IReplayDataReader dataReader, IThreadPoolCounter counter, double speedMultiplier)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (dataReader == null)
                throw new ArgumentNullException(nameof(dataReader));
            if (counter == null)
                throw new ArgumentNullException(nameof(counter));

            _timer = timer;
            _dataReader = dataReader;
            _counter = counter;
            _speedMultiplier = speedMultiplier;
        }

        public IScheduler Create(IReplayScenarioHandler scenarioHandler, int threadId)
        {
            IScheduler scheduler = new ReplayScheduler(_timer, scenarioHandler, _dataReader, _counter, _speedMultiplier, threadId);

            return scheduler;
        }
    }
}