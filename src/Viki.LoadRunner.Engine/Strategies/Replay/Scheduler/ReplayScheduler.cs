using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Scheduler
{
    public class ReplayScheduler : IScheduler
    {
        private readonly ITimer _timer;
        private readonly IReplayScenarioHandler _scenarioHandler;
        private readonly IReplayDataReader _dataReader;
        private readonly double _speedMultiplier;

        private readonly TimeSpan _oneSecond = TimeSpan.FromSeconds(1);

        public ReplayScheduler(ITimer timer, IReplayScenarioHandler scenarioHandler, IReplayDataReader dataReader, double speedMultiplier)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (scenarioHandler == null)
                throw new ArgumentNullException(nameof(scenarioHandler));
            if (dataReader == null)
                throw new ArgumentNullException(nameof(dataReader));
            if (speedMultiplier <= 0)
                throw new ArgumentException("speedMultiplier must be above 0", nameof(speedMultiplier));

            _timer = timer;
            _scenarioHandler = scenarioHandler;
            _dataReader = dataReader;
            _speedMultiplier = speedMultiplier;

        }

        public void WaitNext(ref bool stop)
        {
            DataItem dataItem = _dataReader.Next();

            if (dataItem != null)
            {
                _scenarioHandler.SetData(dataItem.Value);

                TimeSpan adjustedTarget = TimeSpan.FromTicks((long)(dataItem.TimeStamp.Ticks / _speedMultiplier));

                SemiWait(adjustedTarget, ref stop);
            }
            else
            {
                stop = true;
            }
        }

        private void SemiWait(TimeSpan target, ref bool stop)
        {
            TimeSpan delay = CalculateDelay(target);
            while (delay > _oneSecond && stop == false)
            {
                Thread.Sleep(_oneSecond);
                delay = CalculateDelay(target);
            }
            if (delay > TimeSpan.Zero && stop == false)
                Thread.Sleep(delay);
        }

        private TimeSpan CalculateDelay(TimeSpan target)
        {
            TimeSpan current = _timer.Value;

            TimeSpan delay = target - current;

            //Console.WriteLine($"Current:{current} Target:{target} Delay:{delay}");

            return delay;
        }
    }
}