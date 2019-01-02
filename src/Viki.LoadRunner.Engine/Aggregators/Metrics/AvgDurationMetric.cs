using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class AvgDurationMetric : IMetric
    {
        private readonly FlexiRow<string, AverageTimeCalculator> _row = new FlexiRow<string, AverageTimeCalculator>(() => new AverageTimeCalculator());
        private readonly string[] _ignoredCheckpoints;

        public AvgDurationMetric(params string[] ignoredCheckpoints)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = ignoredCheckpoints.Union(Checkpoint.NotMeassuredCheckpoints).ToArray();
        }

        IMetric<IResult> IMetric<IResult>.CreateNew()
        {
            return new AvgDurationMetric(_ignoredCheckpoints);
        }

        void IMetric<IResult>.Add(IResult result)
        {
            ICheckpoint[] checkpoints = result.Checkpoints;
            for (int i = 0, j = checkpoints.Length - 1; i < j; i++)
            {
                ICheckpoint checkpoint = checkpoints[i];
                if (checkpoint.Error == null && _ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = "Avg: " + checkpoint.Name;
                    TimeSpan momentDiff = checkpoint.Diff(checkpoints[i + 1]);

                    _row[key].AddSample(momentDiff);
                }
            }
        }

        string[] IMetric<IResult>.ColumnNames => _row.Keys.ToArray();
        object[] IMetric<IResult>.Values => _row.Values.Select(v => (object)Convert.ToInt64(v.GetAverage().TotalMilliseconds)).ToArray();
    }

    public class AverageTimeCalculator
    {
        private int _sampleCount = 0;
        private TimeSpan _timeSum = TimeSpan.Zero;

        public void AddSample(TimeSpan timeSpan)
        {
            _sampleCount++;
            _timeSum += timeSpan;
        }

        public TimeSpan GetAverage()
        {
            return TimeSpan.FromTicks(_timeSum.Ticks /_sampleCount);
        }
    }
}