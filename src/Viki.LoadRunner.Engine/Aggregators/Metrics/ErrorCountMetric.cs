using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class ErrorCountMetric : IMetric
    {
        private readonly FlexiGrid<string, int> _grid = new FlexiGrid<string, int>((() => default(int))); 

        public IMetric CreateNew()
        {
            return new ErrorCountMetric();
        }

        public void Add(TestContextResult result)
        {
            foreach (Checkpoint checkpoint in result.Checkpoints)
            {
                string key = checkpoint.CheckpointName + " Errors";
                _grid[key] += checkpoint.Error != null ? 1 : 0;

            }
        }

        public string[] ColumnNames => _grid.Select(i => i.Key).ToArray();
        public object[] Values => _grid.Select(i => (object)i.Value).ToArray();
    }
}