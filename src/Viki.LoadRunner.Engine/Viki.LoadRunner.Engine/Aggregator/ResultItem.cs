using System;
using System.Runtime.Serialization;

namespace Viki.LoadRunner.Engine.Aggregator
{
    [Serializable]
    public class ResultItem
    {
        public ResultItem(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;

            _summedTime = new TimeSpan();
            Min = TimeSpan.MaxValue;
            Max = TimeSpan.MinValue;
        }

        public void AddTimeMeassure(TimeSpan duration)
        {
            _summedTime += duration;
            Count++;

            if (Min > duration)
                Min = duration;

            if (Max < duration)
                Max = duration;
        }

        [DataMember]
        public readonly string Name;

        [DataMember]
        public TimeSpan Min { get; private set; }

        [DataMember]
        public TimeSpan Max { get; private set; }

        [DataMember]
        public TimeSpan Average => TimeSpan.FromMilliseconds(_summedTime.TotalMilliseconds / Count);

        [DataMember]
        public double CountPerSecond => (Count/_summedTime.TotalMilliseconds) * 1000.0;

        [DataMember]
        public int Count { get; private set; }

        private TimeSpan _summedTime;

    }
}