using System;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class MinDurationMetric : MultiMetricBase<long>
    {
        private readonly string[] _ignoredCheckpoints;

        private static readonly Checkpoint BlankCheckpoint = new Checkpoint("", TimeSpan.Zero);

        public MinDurationMetric(params string[] ignoredCheckpoints)
            : base(() => default(long))
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = ignoredCheckpoints;
        }

        protected override IMetric CreateNewMetric()
        {
            return new MinDurationMetric(_ignoredCheckpoints);
        }

        protected override void AddResult(TestContextResult result)
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
    }
}