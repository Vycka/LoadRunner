using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class GlobalTimerMinValueMetric : IMetric
    {
        private TimeSpan _minValue = TimeSpan.MaxValue;

        public GlobalTimerMinValueMetric(string name = "Timer Min")
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            ColumnNames = new[] { name };
        }

        public IMetric<IResult> CreateNew()
        {
            return new GlobalTimerMinValueMetric(ColumnNames[0]);
        }

        public void Add(IResult data)
        {
            if (data.IterationStarted < _minValue)
                _minValue = data.IterationStarted;
        }

        public string[] ColumnNames { get; }
        public object[] Values => new object[] { _minValue };
    }
}