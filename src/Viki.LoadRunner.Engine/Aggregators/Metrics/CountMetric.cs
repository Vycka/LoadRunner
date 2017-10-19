using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class CountMetric : IMetric
    {
        private readonly FlexiRow<string, int> _row = new FlexiRow<string, int>((() => default(int)));
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
                    _row[key]++;
                }
            }
        }

        string[] IMetric.ColumnNames => _row.Keys.ToArray();
        object[] IMetric.Values => _row.Values.Select(v => (object)v).ToArray();
    }
}