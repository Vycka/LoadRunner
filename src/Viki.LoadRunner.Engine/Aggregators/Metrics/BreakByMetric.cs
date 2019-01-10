using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    /// <summary>
    /// BreakByMetric allows additional data slicing by provided sub-dimension.
    /// </summary>
    public class BreakByMetric : IMetric
    {
        private readonly MetricsHandler<IResult> _metricsTemplate;
        private readonly IDimension _subDimension;

        private readonly FlexiRow<string, IMetric<IResult>> _metricAggregates;

        public BreakByMetric(IDimension subDimension, params IMetric<IResult>[] actualMetrics)
            : this(subDimension, new MetricsHandler<IResult>(actualMetrics))
        {
        }

        private BreakByMetric(IDimension subDimension, MetricsHandler<IResult> metricsTemplate)
        {
            if (subDimension == null)
                throw new ArgumentNullException(nameof(subDimension));

            _metricsTemplate = metricsTemplate;
            _subDimension = subDimension;

            _metricAggregates = new FlexiRow<string, IMetric<IResult>>(((IMetric<IResult>)_metricsTemplate).CreateNew);
        }

        IMetric<IResult> IMetric<IResult>.CreateNew()
        {
            return new BreakByMetric(_subDimension, _metricsTemplate);
        }

        void IMetric<IResult>.Add(IResult result)
        {
            string key = _subDimension.GetKey(result);

            _metricAggregates[key].Add(result);
        }

        private IEnumerable<string> BuildColumnNames()
        {
            foreach (KeyValuePair<string, IMetric<IResult>> pair in _metricAggregates)
            {
                string prefix = pair.Key;

                foreach (string columnName in pair.Value.ColumnNames)
                {
                    yield return String.Concat(prefix, ": ", columnName);
                }
            }
        }

        private IEnumerable<object> BuildValues()
        {
            return _metricAggregates.SelectMany(kv => kv.Value.Values);
        }

        string[] IMetric<IResult>.ColumnNames => BuildColumnNames().ToArray();
        object[] IMetric<IResult>.Values => BuildValues().ToArray();
    }
}