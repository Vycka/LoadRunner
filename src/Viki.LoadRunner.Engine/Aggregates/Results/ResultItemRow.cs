using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates.Results
{
    public class ResultItemRow : ResultItem
    {
        private readonly List<Exception> _errors;

        private TimeSpan _summedMomentTime;
        private TimeSpan _summedTotalTime;

        private DateTime _firsIterationBeginTime;
        private DateTime _lastIterationEndTime;

        public ResultItemRow(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;

            _errors = new List<Exception>();
            _summedMomentTime = new TimeSpan();
            MomentMin = TimeSpan.MaxValue;
            MomentMax = TimeSpan.MinValue;
            TotalMin = TimeSpan.MaxValue;
            TotalMax = TimeSpan.MinValue;
            _firsIterationBeginTime = DateTime.MaxValue;
            _lastIterationEndTime = DateTime.MinValue;
            Count = 0;
        }

        public void AggregateResult(TimeSpan momentDuration, Checkpoint checkpoint, TestContextResult resultContext)
        {
            Count++;

            if (checkpoint.Error != null)
                _errors.Add(checkpoint.Error);

            _summedMomentTime += momentDuration;
            _summedTotalTime += checkpoint.TimePoint;

            if (MomentMin > momentDuration)
                MomentMin = momentDuration;

            if (MomentMax < momentDuration)
                MomentMax = momentDuration;

            if (TotalMin > checkpoint.TimePoint)
                TotalMin = checkpoint.TimePoint;

            if (TotalMax < checkpoint.TimePoint)
                TotalMax = checkpoint.TimePoint;

            if (_firsIterationBeginTime > resultContext.IterationStarted)
                _firsIterationBeginTime = resultContext.IterationStarted;

            if (_lastIterationEndTime < resultContext.IterationFinished)
                _lastIterationEndTime = resultContext.IterationFinished;
        }
         

        public IReadOnlyList<Exception> GetErrors() => _errors;

        public DateTime GetFirstIterationBeginTime() => _firsIterationBeginTime;
        public DateTime GetLastIterationEndTime() => _lastIterationEndTime;

        public void SetErrors(IEnumerable<Exception> errors)
        {
            _errors.Clear();
            _errors.AddRange(errors);
        }

        public readonly string Name;

        public TimeSpan MomentMin { get; private set; }

        public TimeSpan MomentMax { get; private set; }

        public TimeSpan MomentAverage => TimeSpan.FromMilliseconds(_summedMomentTime.TotalMilliseconds / Count);

        public TimeSpan TotalMin { get; private set; }

        public TimeSpan TotalMax { get; private set; }

        public TimeSpan TotalAverage => TimeSpan.FromMilliseconds(_summedTotalTime.TotalMilliseconds / Count);

        public double ActualCountPerSecond => Count / ((_lastIterationEndTime - _firsIterationBeginTime).TotalMilliseconds / 1000.0);

        public int Count { get; private set; }

        public int ErrorCount => _errors.Count;
    }
}