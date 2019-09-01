using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Viki.LoadRunner.Engine.Aggregators.Utils;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    /// <summary>
    /// Select only subset of rows which will be used to aggregate with provided metrics.
    /// </summary>
    public class FilterMetric<T> : IMetric<T>
    {
        private readonly BoolSelectorDelegate<T> _selector;
        private readonly MetricsTemplate<T> _template;
        private readonly IMetric<T> _metric;

        /// <summary>
        /// Select only subset of rows which will be used to aggregate with provided metrics.
        /// </summary>
        /// <param name="selector">Return true for the data which should be aggregated</param>
        /// <param name="metrics">Metrics to be used for filtered aggregation</param>
        public FilterMetric(BoolSelectorDelegate<T> selector, params IMetric<T>[] metrics)
            : this(selector, new MetricsTemplate<T>(metrics))
        {
        }

        private FilterMetric(BoolSelectorDelegate<T> selector, MetricsTemplate<T> template)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _template = template ?? throw new ArgumentNullException(nameof(template));

            _metric = _template.Create();
        }

        IMetric<T> IMetric<T>.CreateNew()
        {
            return new FilterMetric<T>(_selector, _template);
        }

        void IMetric<T>.Add(T data)
        {
            if (_selector(data))
                _metric.Add(data);
        }

        string[] IMetric<T>.ColumnNames => _metric.ColumnNames;
        object[] IMetric<T>.Values => _metric.Values;
    }
}