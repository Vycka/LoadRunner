using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
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

        IMetric<IResult> IMetric<IResult>.CreateNew()
        {
            return new CountMetric(_ignoredCheckpoints);
        }

        void IMetric<IResult>.Add(IResult result)
        {
            ICheckpoint[] checkpoints = result.Checkpoints;
            for (int i = 0, j = checkpoints.Length; i < j; i++)
            {
                ICheckpoint checkpoint = checkpoints[i];
                if (checkpoint.Error == null && _ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = "Count: " + checkpoint.Name;
                    _row[key]++;
                }
            }
        }

        string[] IMetric<IResult>.ColumnNames => _row.Keys.ToArray();
        object[] IMetric<IResult>.Values => _row.Values.Select(v => (object)v).ToArray();
    }
}