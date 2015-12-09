using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class MaxDurationMetric : IMetric
    {
        private readonly FlexiGrid<string, long> _grid = new FlexiGrid<string, long>(() => long.MinValue);
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
            Checkpoint previousCheckpoint = BlankCheckpoint;
            foreach (Checkpoint checkpoint in result.Checkpoints)
            {
                if (_ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = "Max: " + checkpoint.Name;
                    TimeSpan momentDiff = TimeSpan.FromTicks(checkpoint.TimePoint.Ticks - previousCheckpoint.TimePoint.Ticks);

                    if (_grid[key] < momentDiff.TotalMilliseconds)
                        _grid[key] = Convert.ToInt64(momentDiff.TotalMilliseconds);

                    previousCheckpoint = checkpoint;
                }
            }
        }

        public string[] ColumnNames => _grid.Keys.ToArray();
        public object[] Values => _grid.Values.Select(v => (object)v).ToArray();

    }
}