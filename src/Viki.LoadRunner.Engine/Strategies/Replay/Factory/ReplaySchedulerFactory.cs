using System;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scheduler;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Factory
{
    public class ReplaySchedulerFactory : IReplaySchedulerFactory
    {
        private readonly ITimer _timer;
        private readonly IReplayScenarioHandler _scenarioHandler;
        private readonly IReplayDataReader _dataReader;
        private readonly double _speedMultiplier;


        public ReplaySchedulerFactory(ITimer timer, IReplayDataReader dataReader, double speedMultiplier)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (dataReader == null)
                throw new ArgumentNullException(nameof(dataReader));

            _timer = timer;
            _dataReader = dataReader;
            _speedMultiplier = speedMultiplier;
        }

        public IScheduler Create(IReplayScenarioHandler scenarioHandler)
        {
            IScheduler scheduler = new ReplayScheduler(_timer, _scenarioHandler, _dataReader, _speedMultiplier);

            return scheduler;
        }
    }
}