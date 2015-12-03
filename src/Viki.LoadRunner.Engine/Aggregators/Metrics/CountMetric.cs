using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class CountMetric : IMetric
    {
        private readonly FlexiGrid<string, int> _grid = new FlexiGrid<string, int>((() => default(int)));

        public IMetric CreateNew()
        {
            return new CountMetric();
        }

        public void Add(TestContextResult result)
        {
            foreach (Checkpoint checkpoint in result.Checkpoints)
            {
                string key = checkpoint.CheckpointName + " Count";
                _grid[key]++;
            }
        }

        public string[] ColumnNames => _grid.Keys.ToArray();
        public object[] Values => _grid.Values.Select(v => (object)v).ToArray();
    }
}