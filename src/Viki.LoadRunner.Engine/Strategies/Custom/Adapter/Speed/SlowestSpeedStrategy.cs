using System;
using System.Runtime.CompilerServices;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Speed
{
    public class SlowestSpeedStrategy : ISpeedStrategy
    {
        private readonly ITimer _timer;
        private readonly ISpeedStrategy[] _strategies;
        private readonly ConditionalWeakTable<ISchedule, ScheduleTable> _schedules;

        public static SlowestSpeedStrategy Create(ITimer timer, ISpeedStrategy[] strategies)
        {
            return new SlowestSpeedStrategy(timer, strategies);
        }

        protected SlowestSpeedStrategy(ITimer timer, ISpeedStrategy[] strategies)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (strategies == null)
                throw new ArgumentNullException(nameof(strategies));
            if (strategies.Length == 0)
                throw new ArgumentException("At least one strategy has to be provided", nameof(strategies));

            _timer = timer;
            _strategies = strategies;
            _schedules = new ConditionalWeakTable<ISchedule, ScheduleTable>();
        }

        public void Setup(ITestState state)
        {
            for (int i = 0; i < _strategies.Length; i++)
            {
                _strategies[i].Setup(state);
            }
        }

        public void Next(IIterationId id, ISchedule target)
        {
            ScheduleTable table = GetScheduleTable(target);

            ISchedule schedule = null;
            do
            {
                schedule = table.Schedules[table.ReadPosition];
                _strategies[table.ReadPosition].Next(id, schedule);

                if (schedule.Action == ScheduleAction.Idle)
                    break;

                table.ReadPosition++;
            } while (table.ReadPosition < _strategies.Length);

            if (table.ReadPosition == _strategies.Length && schedule.Action == ScheduleAction.Execute)
            {
                table.ReadPosition = 0;
                target.Action = ScheduleAction.Execute;
                target.At = MaxTarget(table.Schedules);
            }
            else
            {
                target.Action = ScheduleAction.Idle;
                target.At = schedule.At;
            }
        }

        private static TimeSpan MaxTarget(ISchedule[] schedules)
        {
            TimeSpan result = TimeSpan.MinValue;
            for (int i = 0; i < schedules.Length; i++)
            {
                if (result < schedules[i].At)
                    result = schedules[i].At;
            }

            return result;
        }

        public void HeartBeat(ITestState state)
        {
            for (int i = 0; i < _strategies.Length; i++)
            {
                _strategies[i].HeartBeat(state);
            }
        }

        private ScheduleTable GetScheduleTable(ISchedule key)
        {
            ScheduleTable result;
            if (_schedules.TryGetValue(key, out result) == false)
            {
                result = new ScheduleTable(_timer, _strategies.Length);
                _schedules.Add(key, result);
            }

            return result;
        }

        public void ThreadFinished(IIterationId id, ISchedule scheduler)
        {
            ScheduleTable table = GetScheduleTable(scheduler);
            for (int i = 0; i < _strategies.Length; i++)
            {
                _strategies[i].ThreadFinished(id, table.Schedules[i]);
            }
        }
    }
}