using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Aggregates.Utils;

namespace Viki.LoadRunner.Engine.Aggregates.Results
{
    public class ResultItemRow 
    {
        private readonly TimeSpan _summedMomentTime;
        private readonly TimeSpan _summedTotalTime;

        private List<Exception> _errors;

        private readonly DateTime _firsIterationBeginTime;
        private readonly DateTime _lastIterationEndTime;

        public readonly string Name;

        public TimeSpan MomentMin;
        public TimeSpan MomentMax;
        public TimeSpan MomentAverage => TimeSpan.FromMilliseconds(_summedMomentTime.TotalMilliseconds / Count);

        public TimeSpan TotalMin;
        public TimeSpan TotalMax;
        public TimeSpan TotalAverage => TimeSpan.FromMilliseconds(_summedTotalTime.TotalMilliseconds / Count);

        public double ActualCountPerSecond => Count / ((_lastIterationEndTime - _firsIterationBeginTime).TotalMilliseconds / 1000.0);

        public int Count;

        public int ErrorCount => _errors.Count;

        public ResultItemRow(AggregatedCheckpoint aggregator)
        {
            _summedMomentTime = aggregator.SummedMomentTime;
            _summedTotalTime = aggregator.SummedTotalTime;

            _firsIterationBeginTime = aggregator.FirsIterationBeginTime;
            _lastIterationEndTime = aggregator.LastIterationEndTime;

            _errors = aggregator.Errors;

            Name = aggregator.Name;

            MomentMin = aggregator.MomentMin;
            MomentMax = aggregator.MomentMax;

            TotalMin = aggregator.TotalMin;
            TotalMax = aggregator.TotalMax;

            Count = aggregator.Count;
        }
        
        public List<Exception> GetErrors() => _errors;

        public DateTime GetFirstIterationBeginTime() => _firsIterationBeginTime;
        public DateTime GetLastIterationEndTime() => _lastIterationEndTime;

        public void SetErrors(List<Exception> errors)
        {
            _errors = errors;
        }
    }
}