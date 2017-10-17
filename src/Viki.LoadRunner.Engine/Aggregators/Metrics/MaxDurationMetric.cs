using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class MaxDurationMetric : IMetric
    {
        private readonly FlexiRow<string, long> _row = new FlexiRow<string, long>(() => long.MinValue);
        private readonly string[] _ignoredCheckpoints;

        private static readonly Checkpoint BlankCheckpoint = new Checkpoint("", TimeSpan.Zero);

        public MaxDurationMetric(params string[] ignoredCheckpoints)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = ignoredCheckpoints;
        }

        public IMetric CreateNew()
        {
            return new MaxDurationMetric(_ignoredCheckpoints);
        }

        public void Add(IResult result)
        {
            ICheckpoint previousCheckpoint = BlankCheckpoint;
            foreach (ICheckpoint checkpoint in result.Checkpoints)
            {
                if (_ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = "Max: " + checkpoint.Name;
                    TimeSpan momentDiff = TimeSpan.FromTicks(checkpoint.TimePoint.Ticks - previousCheckpoint.TimePoint.Ticks);

                    if (_row[key] < momentDiff.TotalMilliseconds)
                        _row[key] = Convert.ToInt64(momentDiff.TotalMilliseconds);

                    previousCheckpoint = checkpoint;
                }
            }
        }

        public string[] ColumnNames => _row.Keys.ToArray();
        public object[] Values => _row.Values.Select(v => (object)v).ToArray();

    }
}