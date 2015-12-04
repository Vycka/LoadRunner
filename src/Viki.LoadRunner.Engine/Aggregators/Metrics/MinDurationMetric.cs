using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class MinDurationMetric : IMetric
    {
        private readonly FlexiGrid<string, long> _grid = new FlexiGrid<string, long>(() => long.MaxValue);
        private readonly string[] _ignoredCheckpoints;

        private static readonly Checkpoint BlankCheckpoint = new Checkpoint("", TimeSpan.Zero);

        public MinDurationMetric(params string[] ignoredCheckpoints)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = ignoredCheckpoints;
        }

        public IMetric CreateNew()
        {
            return new MinDurationMetric(_ignoredCheckpoints);
        }

        public void Add(TestContextResult result)
        {
            Checkpoint previousCheckpoint = BlankCheckpoint;
            foreach (Checkpoint checkpoint in result.Checkpoints)
            {
                if (_ignoredCheckpoints.All(name => name != checkpoint.CheckpointName))
                {
                    string key = "Min: " + checkpoint.CheckpointName;
                    TimeSpan momentDiff =
                        TimeSpan.FromTicks(checkpoint.TimePoint.Ticks - previousCheckpoint.TimePoint.Ticks);

                    if (_grid[key] > momentDiff.TotalMilliseconds)
                        _grid[key] = Convert.ToInt64(momentDiff.TotalMilliseconds);
                }
                previousCheckpoint = checkpoint;
            }
            
        }

        public string[] ColumnNames => _grid.Keys.ToArray();
        public object[] Values => _grid.Values.Select(v => (object)v).ToArray();
    }
}