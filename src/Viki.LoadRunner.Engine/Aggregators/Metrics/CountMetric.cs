using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

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

        void IMetric.Add(IResult result)
        {
            foreach (ICheckpoint checkpoint in result.Checkpoints)
            {
                if (_ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = "Count: " + checkpoint.Name;
                    _grid[key]++;
                }
            }
        }

        string[] IMetric.ColumnNames => _grid.Keys.ToArray();
        object[] IMetric.Values => _grid.Values.Select(v => (object)v).ToArray();
    }
}