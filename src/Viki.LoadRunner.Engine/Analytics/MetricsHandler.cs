using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics
{
    namespace Viki.LoadRunner.Engine.Aggregators.Utils
    {
        public class MetricsHandler<T> : IMetric<T>
        {
            private readonly IMetric<T>[] _metrics;

            /// <summary>
            /// MetricsHandler acts as signle metric, but it wraps multiple provided metrics and makes them work as one
            /// </summary>
            /// <param name="metricTemplates"></param>
            public MetricsHandler(IEnumerable<IMetric<T>> metricTemplates)
            {
                if (metricTemplates == null)
                    throw new ArgumentNullException(nameof(metricTemplates));

                _metrics = metricTemplates.ToArray();
            }

            IMetric<T> IMetric<T>.CreateNew()
            {
                return new MetricsHandler<T>(_metrics.Select(m => m.CreateNew()));
            }

            void IMetric<T>.Add(T result)
            {
                Array.ForEach(_metrics, m => m.Add(result));
            }

            string[] IMetric<T>.ColumnNames => _metrics.SelectMany(m => m.ColumnNames).ToArray();
            object[] IMetric<T>.Values => _metrics.SelectMany(m => m.Values).ToArray();
        }
    }
}
