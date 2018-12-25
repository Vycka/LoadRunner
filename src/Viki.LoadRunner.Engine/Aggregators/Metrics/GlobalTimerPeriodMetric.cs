using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class GlobalTimerPeriodMetric : IMetric
    {
        TimeSpan _min = TimeSpan.MaxValue;
        TimeSpan _max = TimeSpan.MinValue;

        public GlobalTimerPeriodMetric(string name = "TotalDuration")
        {
            ColumnNames = new[] {name};
        }

        public IMetric<IResult> CreateNew()
        {
            return new GlobalTimerPeriodMetric(ColumnNames[0]);
        }

        public void Add(IResult data)
        {
            if (_min > data.IterationStarted)
                _min = data.IterationStarted;

            if (_max < data.IterationFinished)
                _max = data.IterationFinished;
        }

        public string[] ColumnNames { get; }
        public object[] Values => new object[] { (long)((_max - _min).TotalMilliseconds) };
    }
}