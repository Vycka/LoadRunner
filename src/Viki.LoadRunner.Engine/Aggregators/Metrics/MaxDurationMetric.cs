using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class MaxDurationMetric : IMetric
    {
        readonly FlexiGrid<string, TimeSpan> _grid = new FlexiGrid<string, TimeSpan>(() => TimeSpan.MinValue);

        public IMetric CreateNew()
        {
            return new MaxDurationMetric();
        }

        public void Add(TestContextResult result)
        {
            foreach (Checkpoint checkpoint in result.Checkpoints)
            {
                string key = checkpoint.CheckpointName + " Max";

                if (_grid[key] < checkpoint.TimePoint)
                    _grid[key] = checkpoint.TimePoint;
            }

        }

        public string[] ColumnNames => _grid.Select(i => i.Key).ToArray();
        public object[] Values => _grid.Select(i => (object)i.Value).ToArray();

    }
}