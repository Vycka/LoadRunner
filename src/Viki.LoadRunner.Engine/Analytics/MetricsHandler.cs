using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics
{
    namespace Viki.LoadRunner.Engine.Aggregators.Utils
    {
        public class MetricsHandler<T>
        {
            private readonly IMetric<T>[] _metrics;

            /// <summary>
            /// MetricsHandler acts as single metric, but it wraps multiple provided metrics and makes them work as one
            /// </summary>
            /// <param name="metricTemplates"></param>
            public MetricsHandler(IEnumerable<IMetric<T>> metricTemplates)
            {
                if (metricTemplates == null)
                    throw new ArgumentNullException(nameof(metricTemplates));

                _metrics = metricTemplates.ToArray();
            }

            public MetricsHandler<T> Create()
            {
                return new MetricsHandler<T>(_metrics.Select(m => m.CreateNew()));
            }

            public void Add(T result)
            {
                for (int i = 0; i < _metrics.Length; i++)
                {
                    _metrics[i].Add(result);
                }
            }

            public IEnumerable<Val> Export()
            {
                var keys = _metrics.SelectMany(m => m.ColumnNames).ToArray();
                var values = _metrics.SelectMany(m => m.Values).ToArray();

                return keys.Select((k, i) => new Val(k, values[i]));
            }
        }
    }

    public struct Val
    {
        public Val(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key;
        public object Value;
    }
}
