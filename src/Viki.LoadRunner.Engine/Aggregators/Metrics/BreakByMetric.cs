using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    /// <summary>
    /// BreakByMetric allows additional data slicing by provided sub-dimension.
    /// </summary>
    public class BreakByMetric : IMetric
    {
        private readonly MetricMultiplexer _metricsTemplate;
        private readonly IDimension _subDimension;

        private readonly FlexiRow<string, IMetric> _metricAggregates;

        public BreakByMetric(IDimension subDimension, params IMetric[] actualMetrics)
            : this(subDimension, new MetricMultiplexer(actualMetrics))
        {
        }

        private BreakByMetric(IDimension subDimension, MetricMultiplexer metricsTemplate)
        {
            if (subDimension == null)
                throw new ArgumentNullException(nameof(subDimension));

            _metricsTemplate = metricsTemplate;
            _subDimension = subDimension;

            _metricAggregates = new FlexiRow<string, IMetric>(((IMetric)_metricsTemplate).CreateNew);
        }

        IMetric IMetric.CreateNew()
        {
            return new BreakByMetric(_subDimension, _metricsTemplate);
        }

        void IMetric.Add(IResult result)
        {
            string key = _subDimension.GetKey(result);

            _metricAggregates[key].Add(result);
        }

        private IEnumerable<string> BuildColumnNames()
        {
            foreach (KeyValuePair<string, IMetric> pair in _metricAggregates)
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

        string[] IMetric.ColumnNames => BuildColumnNames().ToArray();
        object[] IMetric.Values => BuildValues().ToArray();


    }
}