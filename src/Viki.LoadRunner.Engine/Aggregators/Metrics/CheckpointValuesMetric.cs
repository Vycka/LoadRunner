using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    /// <summary>
    /// List checkpoints and their durations.
    /// Should not be used in aggregations where multiple items fall under same dimensions
    /// Otherwise information from last received results row will be applied
    /// </summary>
    public class CheckpointValuesMetric : MultiMetricBase<long> // Rename to CheckpointDurations or smth more meaningful
    {
        private readonly string _prefix;
        private readonly string[] _ignoredCheckpoints;


        public CheckpointValuesMetric(params string[] ignoredCheckpoints)
            : this("Kpi: ", ignoredCheckpoints)
        {
        }


        public CheckpointValuesMetric(string prefix, params string[] ignoredCheckpoints)
            : base(() => 0)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));

            _ignoredCheckpoints = ignoredCheckpoints.ToArray();
        }

        protected override IMetric<IResult> CreateNewMetric()
        {
            return new CheckpointValuesMetric(_prefix, _ignoredCheckpoints);
        }

        protected override void AddResult(IResult result)
        {
            ICheckpoint[] checkpoints = result.Checkpoints;

            if (checkpoints.Length == 1)
            {
                ICheckpoint checkpoint = checkpoints[0];

                if (_ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = _prefix + checkpoint.Name;
                    _row[key] = 0;
                }
            }
            else
            {
                for (int i = 0, j = checkpoints.Length - 1; i < j; i++)
                {
                    ICheckpoint checkpoint = checkpoints[i];
                    if (_ignoredCheckpoints.All(name => name != checkpoint.Name))
                    {
                        string key = _prefix + checkpoint.Name;
                        TimeSpan momentDiff = checkpoint.Diff(checkpoints[i + 1]);

                        _row[key] = Convert.ToInt64(momentDiff.TotalMilliseconds);
                    }
                }
            }
        }

        public static IEnumerable<Val<TimeSpan>> Parse(ICheckpoint[] checkpoints)
        {
            if (checkpoints.Length == 1)
                yield return new Val<TimeSpan>(checkpoints[0].Name, checkpoints[0].TimePoint);
            else
            {
                int length = checkpoints.Length - 1;
                for (int i = 0; i < length; i++)
                {
                    ICheckpoint checkpoint = checkpoints[i];
                    yield return new Val<TimeSpan>(checkpoint.Name, checkpoint.Diff(checkpoints[i+1]));
                }
            }
        }
    }
}