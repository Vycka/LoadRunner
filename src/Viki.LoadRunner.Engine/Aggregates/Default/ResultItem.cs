using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates.Default
{
    [Serializable]
    public class ResultItem
    {
        public ResultItem(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;

            Errors = new List<Exception>();
            _summedMomentTime = new TimeSpan();
            MomentMin = TimeSpan.MaxValue;
            MomentMax = TimeSpan.MinValue;
            TotalMin = TimeSpan.MaxValue;
            TotalMax = TimeSpan.MinValue;
        }

        public void AggregateResult(TimeSpan momentDuration, Checkpoint checkpoint)
        {
            Count++;

            if (checkpoint.Errors != null)
                Errors.AddRange(checkpoint.Errors);
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
        }

        [DataMember]
        public readonly string Name;

        [DataMember]
        public TimeSpan MomentMin { get; private set; }

        [DataMember]
        public TimeSpan MomentMax { get; private set; }

        [DataMember]
        public TimeSpan MomentAverage => TimeSpan.FromMilliseconds(_summedMomentTime.TotalMilliseconds / Count);

        [DataMember]
        public TimeSpan TotalMin { get; private set; }

        [DataMember]
        public TimeSpan TotalMax { get; private set; }

        [DataMember]
        public TimeSpan TotalAverage => TimeSpan.FromMilliseconds(_summedTotalTime.TotalMilliseconds / Count);


        [DataMember]
        public double CountPerSecond => (Count / _summedMomentTime.TotalMilliseconds) * 1000.0;

        [DataMember]
        public int Count { get; private set; }

        [DataMember]
        public List<Exception> Errors;

        [DataMember]
        public int ErrorCount => Errors.Count;

        [DataMember]
        public double ErrorRate => (1.0 / Count) * ErrorCount;

        private TimeSpan _summedMomentTime;

        private TimeSpan _summedTotalTime;

    }
}