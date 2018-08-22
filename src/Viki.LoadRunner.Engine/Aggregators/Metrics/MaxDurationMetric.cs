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
    public class MaxDurationMetric : IMetric
    {
        private readonly FlexiRow<string, long> _row = new FlexiRow<string, long>(() => long.MinValue);
        private readonly string[] _ignoredCheckpoints;

        public MaxDurationMetric(params string[] ignoredCheckpoints)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = ignoredCheckpoints.Union(Checkpoint.NotMeassuredCheckpoints).ToArray();
        }

        public IMetric<IResult> CreateNew()
        {
            return new MaxDurationMetric(_ignoredCheckpoints);
        }

        public void Add(IResult result)
        {
            ICheckpoint[] checkpoints = result.Checkpoints;
            for (int i = 0, j = checkpoints.Length - 1; i < j; i++)
            {
                ICheckpoint checkpoint = checkpoints[i];
                if (checkpoint.Error == null && _ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = "Max: " + checkpoint.Name;
                    TimeSpan momentDiff = checkpoint.Diff(checkpoints[i + 1]);

                    if (_row[key] < momentDiff.TotalMilliseconds)
                        _row[key] = Convert.ToInt64(momentDiff.TotalMilliseconds);

                }
            }
        }

        public string[] ColumnNames => _row.Keys.ToArray();
        public object[] Values => _row.Values.Select(v => (object)v).ToArray();

    }
}