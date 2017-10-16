using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scenario;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class MinDurationMetric : MultiMetricBase<long>
    {
        private readonly string[] _ignoredCheckpoints;

        private static readonly Checkpoint BlankCheckpoint = new Checkpoint("", TimeSpan.Zero);

        public MinDurationMetric(params string[] ignoredCheckpoints)
            : base(() => long.MaxValue)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = ignoredCheckpoints;
        }

        protected override IMetric CreateNewMetric()
        {
            return new MinDurationMetric(_ignoredCheckpoints);
        }

        protected override void AddResult(IResult result)
        {
            ICheckpoint previousCheckpoint = BlankCheckpoint;
            foreach (ICheckpoint checkpoint in result.Checkpoints)
            {
                if (_ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = "Min: " + checkpoint.Name;
                    TimeSpan momentDiff =
                        TimeSpan.FromTicks(checkpoint.TimePoint.Ticks - previousCheckpoint.TimePoint.Ticks);

                    if (_row[key] > momentDiff.TotalMilliseconds)
                        _row[key] = Convert.ToInt64(momentDiff.TotalMilliseconds);
                }
                previousCheckpoint = checkpoint;
            }
            
        }
    }
}