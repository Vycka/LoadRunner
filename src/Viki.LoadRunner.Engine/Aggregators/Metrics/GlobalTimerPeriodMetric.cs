using System;
using System.Globalization;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class GlobalTimerPeriodMetric : IMetric
    {
        TimeSpan _min = TimeSpan.MaxValue;
        TimeSpan _max = TimeSpan.MinValue;

        public Func<TimeSpan, object> Formatter = (t) => t.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);

        public GlobalTimerPeriodMetric(string name = "Timer period (ms)")
        {
            ColumnNames = new[] { name };
        }

        public IMetric<IResult> CreateNew()
        {
            return new GlobalTimerPeriodMetric(ColumnNames[0])
            {
                Formatter = Formatter
            };
        }

        public void Add(IResult data)
        {
            if (_min > data.IterationStarted)
                _min = data.IterationStarted;

            if (_max < data.IterationFinished)
                _max = data.IterationFinished;
        }

        public string[] ColumnNames { get; }
        public object[] Values => new [] { Formatter(_max - _min) };
    }
}