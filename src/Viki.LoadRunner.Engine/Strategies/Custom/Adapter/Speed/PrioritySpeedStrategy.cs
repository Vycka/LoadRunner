using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Speed
{
    internal class PrioritySpeedStrategy : ISpeedStrategy
    {
        private readonly IPriorityResolver _strategy;
        private readonly ITimer _timer;
        private readonly ISpeedStrategy[] _strategies;
        private readonly ConditionalWeakTable<ISchedule, ISchedule[]> _schedules;


        public PrioritySpeedStrategy(IPriorityResolver strategy, ITimer timer, ISpeedStrategy[] strategies)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (strategies == null)
                throw new ArgumentNullException(nameof(strategies));
            if (strategies.Length == 0)
                throw new ArgumentException("At least one strategy has to be provided", nameof(strategies));

            _strategy = strategy;
            _timer = timer;
            _strategies = strategies;
            _schedules = new ConditionalWeakTable<ISchedule, ISchedule[]>();
        }

        public void Setup(ITestState state)
        {
            for (int i = 0; i < _strategies.Length; i++)
            {
                _strategies[i].Setup(state);
            }
        }

        public void Next(IIterationState state, ISchedule target)
        {
            ISchedule[] schedules = GetScheduleTable(target);

            TimeSpan time = target.Timer.Value;

            for (int i = 0; i < _strategies.Length; i++)
            {
                if (schedules[i].At < time)
                    _strategies[i].Next(state, schedules[i]);
            }

            _strategy.Apply(schedules, target);
        }

        public void HeartBeat(ITestState state)
        {
            for (int i = 0; i < _strategies.Length; i++)
            {
                _strategies[i].HeartBeat(state);
            }
        }

        private ISchedule[] GetScheduleTable(ISchedule key)
        {
            ISchedule[] result;
            if (_schedules.TryGetValue(key, out result) == false)
            {
                result = Enumerable.Repeat(1, _strategies.Length).Select(i => (ISchedule)new Schedule(_timer)).ToArray();
                _schedules.Add(key, result);
            }

            return result;
        }
    }

    public interface IPriorityResolver
    {
        void Apply(ISchedule[] schedules, ISchedule target);
    }
}

/*
 using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Speed
{
    internal class PrioritySpeedStrategy : ISpeedStrategy
    {
        private readonly IPriorityResolver _strategy;
        private readonly ITimer _timer;
        private readonly ISpeedStrategy[] _strategies;
        private readonly ConditionalWeakTable<ISchedule, ISchedule[]> _schedules;


        public PrioritySpeedStrategy(IPriorityResolver strategy, ITimer timer, ISpeedStrategy[] strategies)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));
            if (strategies == null)
                throw new ArgumentNullException(nameof(strategies));
            if (strategies.Length == 0)
                throw new ArgumentException("At least one strategy has to be provided", nameof(strategies));

            _strategy = strategy;
            _timer = timer;
            _strategies = strategies;
            _schedules = new ConditionalWeakTable<ISchedule, ISchedule[]>();
        }

        public void Setup(ITestState state)
        {
            for (int i = 0; i < _strategies.Length; i++)
            {
                _strategies[i].Setup(state);
            }
        }

        public void Next(IIterationState state, ISchedule target)
        {
            ISchedule[] schedules = GetScheduleTable(target);

            TimeSpan time = target.Timer.Value;

            
            for (int i = 0; i < _strategies.Length; i++)
            {
                if ((target.Action == ScheduleAction.Execute && schedules[i].Action == ScheduleAction.Execute) || (schedules[i].Action == ScheduleAction.Idle && schedules[i].At < time))
                    _strategies[i].Next(state, schedules[i]);
                    

                //if (schedules[i].At < time)
                //    _strategies[i].Next(state, schedules[i]);
            }

            Apply(schedules, target);
            //_strategy.Apply(schedules, target);

            Console.WriteLine($"{state.Iteration.ThreadId}: {String.Join(" ", schedules.Select(s => $"{s.Action}:{s.At:g}"))} --> {target.Action}:{target.At:g}");
        }

        public void HeartBeat(ITestState state)
        {
            for (int i = 0; i < _strategies.Length; i++)
            {
                _strategies[i].HeartBeat(state);
            }
        }

        private ISchedule[] GetScheduleTable(ISchedule key)
        {
            ISchedule[] result;
            if (_schedules.TryGetValue(key, out result) == false)
            {
                result = Enumerable.Repeat(1, _strategies.Length).Select(i => (ISchedule)new Schedule(_timer)).ToArray();
                _schedules.Add(key, result);
            }

            return result;
        }

        public void Apply(ISchedule[] schedules, ISchedule target)
        {
            target.Action = ScheduleAction.Execute;
            target.At = schedules[0].At;

            for (int i = 0; i < schedules.Length; i++)
            {
                ISchedule schedule = schedules[i];

                if (schedule.Action == ScheduleAction.Idle)
                    target.Action = ScheduleAction.Idle;

                if (schedule.At > target.At)
                    target.At = schedule.At;
            }
        }
    }

    public interface IPriorityResolver
    {
        void Apply(ISchedule[] schedules, ISchedule target);
    }
}
 */
