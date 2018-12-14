using System;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    /// <summary>
    /// Simple Checkpoint with its duration KPI.
    /// Should not be used in aggregations where multiple items fall under same dimensions
    /// Otherwise information from last received results row will be applied
    /// </summary>
    public class KpiDurationMetric : MultiMetricBase<long> // Rename to CheckpointDurations or smth more meaningful
    {
        private readonly string[] _ignoredCheckpoints;

        public KpiDurationMetric(params string[] ignoredCheckpoints)
            : base(() => 0)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = ignoredCheckpoints.Union(Checkpoint.NotMeassuredCheckpoints).ToArray();
        }

        protected override IMetric<IResult> CreateNewMetric()
        {
            return new KpiDurationMetric(_ignoredCheckpoints);
        }

        protected override void AddResult(IResult result)
        {
            ICheckpoint[] checkpoints = result.Checkpoints;
            for (int i = 0, j = checkpoints.Length - 1; i < j; i++)
            {
                ICheckpoint checkpoint = checkpoints[i];
                if (_ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = "Kpi: " + checkpoint.Name;
                    TimeSpan momentDiff = checkpoint.Diff(checkpoints[i + 1]);

                    _row[key] = Convert.ToInt64(momentDiff.TotalMilliseconds);
                }
            }
        }
    }
}