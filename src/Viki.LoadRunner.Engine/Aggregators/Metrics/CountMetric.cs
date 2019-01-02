using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    /// <summary>
    /// Count successful iterations.
    /// </summary>
    public class CountMetric : IMetric
    {
        private readonly FlexiRow<string, int> _row = new FlexiRow<string, int>((() => default(int)));
        private readonly string[] _ignoredCheckpoints;
        public string Prefix = "Count: ";

        private readonly Func<int, object> _formatter = t => t;

        public CountMetric(Func<int, object> formatter, params string[] ignoredCheckpoints)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _ignoredCheckpoints = ignoredCheckpoints ?? throw new ArgumentNullException(nameof(ignoredCheckpoints));
        }

        public CountMetric(params string[] ignoredCheckpoints)
        {
            _ignoredCheckpoints = ignoredCheckpoints ?? throw new ArgumentNullException(nameof(ignoredCheckpoints));
        }

        IMetric<IResult> IMetric<IResult>.CreateNew()
        {
            return new CountMetric(_formatter, _ignoredCheckpoints) { Prefix = Prefix };
        }

        void IMetric<IResult>.Add(IResult result)
        {
            ICheckpoint[] checkpoints = result.Checkpoints;
            for (int i = 0, j = checkpoints.Length; i < j; i++)
            {
                ICheckpoint checkpoint = checkpoints[i];
                if (checkpoint.Error == null && _ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = Prefix + checkpoint.Name;
                    _row[key]++;
                }
            }
        }

        string[] IMetric<IResult>.ColumnNames => _row.Keys.ToArray();
        object[] IMetric<IResult>.Values => _row.Values.Select(_formatter).ToArray();
    }
}