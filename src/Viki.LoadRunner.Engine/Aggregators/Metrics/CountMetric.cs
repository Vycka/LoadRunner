using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class CountMetric : IMetric
    {
        private readonly FlexiGrid<string, int> _grid = new FlexiGrid<string, int>((() => default(int)));
        private readonly string[] _ignoredCheckpoints;

        public CountMetric(params string[] ignoredCheckpoints)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = ignoredCheckpoints;
        }

        IMetric IMetric.CreateNew()
        {
            return new CountMetric(_ignoredCheckpoints);
        }

        void IMetric.Add(TestContextResult result)
        {
            foreach (Checkpoint checkpoint in result.Checkpoints)
            {
                if (_ignoredCheckpoints.All(name => name != checkpoint.CheckpointName))
                {
                    string key = "Count: " + checkpoint.CheckpointName;
                    _grid[key]++;
                }
            }
        }

        string[] IMetric.ColumnNames => _grid.Keys.ToArray();
        object[] IMetric.Values => _grid.Values.Select(v => (object)v).ToArray();
    }
}