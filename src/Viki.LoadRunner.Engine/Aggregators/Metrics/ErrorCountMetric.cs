using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class ErrorCountMetric : IMetric
    {
        private readonly bool _includeTotals;
        private readonly FlexiGrid<string, int> _grid = new FlexiGrid<string, int>((() => default(int)));

        public ErrorCountMetric(bool includeTotals = true)
        {
            _includeTotals = includeTotals;

            if (_includeTotals)
                _grid.Touch("Errors: Totals");
        }

        public IMetric CreateNew()
        {
            return new ErrorCountMetric();
        }

        public void Add(TestContextResult result)
        {
            foreach (Checkpoint checkpoint in result.Checkpoints)
            {
                string key = "Errors: " + checkpoint.CheckpointName;

                if (checkpoint.Error != null)
                {
                    _grid[key]++;

                    if (_includeTotals)
                        _grid["Errors: Totals"]++;
                }
            }
        }

        public string[] ColumnNames => _grid.Keys.ToArray();
        public object[] Values => _grid.Values.Select(v => (object)v).ToArray();
    }
}