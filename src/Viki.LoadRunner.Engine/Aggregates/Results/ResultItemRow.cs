using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Aggregates.Aggregates;

namespace Viki.LoadRunner.Engine.Aggregates.Results
{
    public class ResultItemRow 
    {
        private List<Exception> _errors;

        public readonly string Name;

        public TimeSpan MomentMin;
        public TimeSpan MomentMax;
        public TimeSpan MomentAverage;

        public TimeSpan SummedMin;
        public TimeSpan SummedMax;
        public TimeSpan SummedAverage;

        public double SuccessIterationsPerSec;

        public int Count;

        public int ErrorCount => _errors.Count;

        public ResultItemRow(DefaultTestContextResultAggregate testContextResultAggregate, DefaultCheckpointAggregate checkpointAggregate)
        {
            _errors = checkpointAggregate.Errors;

            Count = checkpointAggregate.Count;

            Name = checkpointAggregate.Name;

            MomentMin = checkpointAggregate.MomentMin;
            MomentMax = checkpointAggregate.MomentMax;
            MomentAverage = TimeSpan.FromMilliseconds(checkpointAggregate.SummedMomentTime.TotalMilliseconds / Count);

            SummedMin = checkpointAggregate.TotalMin;
            SummedMax = checkpointAggregate.TotalMax;
            SummedAverage = TimeSpan.FromMilliseconds(checkpointAggregate.SummedTotalTime.TotalMilliseconds/Count);

            SuccessIterationsPerSec = Count / (testContextResultAggregate.IterationEndTime - testContextResultAggregate.IterationBeginTime).TotalMilliseconds * 1000;
        }
        
        public List<Exception> GetErrors() => _errors;

        public void SetErrors(List<Exception> errors)
        {
            _errors = errors;
        }
    }
}