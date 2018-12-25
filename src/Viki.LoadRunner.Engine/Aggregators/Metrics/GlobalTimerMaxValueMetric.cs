using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class GlobalTimerMaxValueMetric : IMetric
    {
        private TimeSpan _maxValue = TimeSpan.MinValue;

        public GlobalTimerMaxValueMetric(string name = "Timer Max")
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
            if (data.IterationFinished > _maxValue)
                _maxValue = data.IterationFinished;
        }

        public string[] ColumnNames { get; }
        public object[] Values => new object[] { _maxValue };
    }
}