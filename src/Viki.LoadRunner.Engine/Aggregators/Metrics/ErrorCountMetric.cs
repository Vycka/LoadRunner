﻿using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class ErrorCountMetric : IMetric
    {
        private readonly bool _includeTotals;
        private readonly FlexiRow<string, int> _row = new FlexiRow<string, int>(() => default(int));

        public ErrorCountMetric(bool includeTotals = true)
        {
            _includeTotals = includeTotals;

            if (_includeTotals)
                _row.Touch("Errors: Totals");
        }

        IMetric<IResult> IMetric<IResult>.CreateNew()
        {
            return new ErrorCountMetric(_includeTotals);
        }

        void IMetric<IResult>.Add(IResult result)
        {

            ICheckpoint[] checkpoints = result.Checkpoints;
            for (int i = 0, j = checkpoints.Length; i < j; i++)
            {
                ICheckpoint checkpoint = checkpoints[i];
                if (checkpoint.Error != null)
                {
                    string key = "Errors: " + checkpoint.Name;

                    _row[key]++;

                    if (_includeTotals)
                        _row["Errors: Totals"]++;
                }
            }
        }

        string[] IMetric<IResult>.ColumnNames => _row.Keys.ToArray();
        object[] IMetric<IResult>.Values => _row.Values.Select(v => (object)v).ToArray();
    }
}