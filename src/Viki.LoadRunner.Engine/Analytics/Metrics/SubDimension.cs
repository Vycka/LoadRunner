using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Viki.LoadRunner.Engine.Aggregators.Utils;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{

    public class SubDimension<T> : IMetric<T>
    {
        private readonly MetricsHandler<T> _metricsHandler;
        private readonly IDimension<T> _subDimension;
        private readonly ColumnNameDelegate _columnNameSelector;

        private readonly FlexiRow<string, IMetric<T>> _row;

        public SubDimension(IDimension<T> subDimension, params IMetric<T>[] actualMetrics)
            : this(subDimension, (d, m) => String.Concat(d, ": ", m), new MetricsHandler<T>(actualMetrics))
        {
        }

        public SubDimension(IDimension<T> subDimension, ColumnNameDelegate columnNameSelector, params IMetric<T>[] actualMetrics)
            : this(subDimension, columnNameSelector, new MetricsHandler<T>(actualMetrics))
        {
        }

        private SubDimension(IDimension<T> subDimension, ColumnNameDelegate columnNameSelector, MetricsHandler<T> metricsHandler)
        {
            _subDimension = subDimension ?? throw new ArgumentNullException(nameof(subDimension));
            _metricsHandler = metricsHandler ?? throw new ArgumentNullException(nameof(metricsHandler));
            _columnNameSelector = columnNameSelector ?? throw new ArgumentNullException(nameof(columnNameSelector));

            _row = new FlexiRow<string, IMetric<T>>(((IMetric<T>)_metricsHandler).CreateNew);
        }

        IMetric<T> IMetric<T>.CreateNew()
        {
            return new SubDimension<T>(_subDimension, _columnNameSelector, _metricsHandler);
        }

        void IMetric<T>.Add(T result)
        {
            string key = _subDimension.GetKey(result);

            _row[key].Add(result);
        }

        private IEnumerable<string> BuildColumnNames()
        {
            foreach (KeyValuePair<string, IMetric<T>> pair in _row)
            {
                string dimensionKey = pair.Key;

                foreach (string metricColumn in pair.Value.ColumnNames)
                {
                    yield return _columnNameSelector(dimensionKey, metricColumn);
                }
            }
        }

        private IEnumerable<object> BuildValues()
        {
            return _row.SelectMany(kv => kv.Value.Values);
        }

        string[] IMetric<T>.ColumnNames => BuildColumnNames().ToArray();
        object[] IMetric<T>.Values => BuildValues().ToArray();

        public delegate string ColumnNameDelegate(string dimensionName, string metricName);
    }
}