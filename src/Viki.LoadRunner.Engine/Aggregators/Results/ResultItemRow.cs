using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Aggregators.Aggregates;

namespace Viki.LoadRunner.Engine.Aggregators.Results
{
    public class ResultItemRow 
    {
        private List<Exception> _errors;
        private int _countDiv => (Count == 0 ? 1 : Count);

        public readonly string Name;

        public TimeSpan MomentMin;
        public TimeSpan MomentMax;
        public TimeSpan MomentAverage;

        public TimeSpan SummedMin;
        public TimeSpan SummedMax;
        public TimeSpan SummedAverage;

        public double SuccessIterationsPerSec;
        public double ErrorRatio => Count == 0 ? 1.0 : 1.0 / (Count + ErrorCount) * ErrorCount;
        public int Count;
        public int ErrorCount => _errors.Count;

        public ResultItemRow(string name, ResultItemRow copyStatsFrom)
        {
            _errors = copyStatsFrom._errors;

            Count = copyStatsFrom.Count;

            Name = name;

            MomentMin = copyStatsFrom.MomentMin;
            MomentMax = copyStatsFrom.MomentMax;
            MomentAverage = copyStatsFrom.MomentAverage;

            SummedMin = copyStatsFrom.SummedMin;
            SummedMax = copyStatsFrom.SummedMax;
            SummedAverage = copyStatsFrom.SummedAverage;

            SuccessIterationsPerSec = copyStatsFrom.SuccessIterationsPerSec;
        }


        public ResultItemRow(DefaultTestContextResultAggregate testContextResultAggregate, DefaultCheckpointAggregate checkpointAggregate)
        {
            _errors = checkpointAggregate.Errors;

            Count = checkpointAggregate.Count;

            Name = checkpointAggregate.Name;

            MomentMin = checkpointAggregate.MomentMin;
            MomentMax = checkpointAggregate.MomentMax;
            MomentAverage = TimeSpan.FromMilliseconds(checkpointAggregate.SummedMomentTime.TotalMilliseconds / _countDiv);

            SummedMin = checkpointAggregate.TotalMin;
            SummedMax = checkpointAggregate.TotalMax;
            SummedAverage = TimeSpan.FromMilliseconds(checkpointAggregate.SummedTotalTime.TotalMilliseconds / _countDiv);

            SuccessIterationsPerSec = Count / (testContextResultAggregate.IterationEndTime - testContextResultAggregate.IterationBeginTime).TotalMilliseconds * 1000;
        }
        
        public List<Exception> GetErrors() => _errors;

        public void SetErrors(List<Exception> errors)
        {
            _errors = errors;
        }
    }
}