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
            private readonly Dictionary<int, PostProcessDelegate> _postProcess;
            private readonly IMetric<T>[] _metrics;

            public MetricsHandler(IEnumerable<IMetric<T>> metricTemplates, Dictionary<int, PostProcessDelegate> postProcess = null)
            {
                if (metricTemplates == null)
                    throw new ArgumentNullException(nameof(metricTemplates));

                _postProcess = postProcess ?? new Dictionary<int, PostProcessDelegate>();
                _metrics = metricTemplates.ToArray();
            }

            public MetricsHandler<T> Create()
            {
                return new MetricsHandler<T>(_metrics.Select(m => m.CreateNew()), _postProcess);
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
                IEnumerable<string[]> keys = _metrics.Select(m => m.ColumnNames);
                object[][] values = _metrics.Select(m => m.Values).ToArray();

                IEnumerable<Val> result = keys
                    .SelectMany((k, i) =>
                    {
                        IEnumerable<Val> metricResult = k.Select((kk, ii) => new Val(kk, values[i][ii]));

                        if (_postProcess.TryGetValue(i, out var processor))
                            metricResult = processor(metricResult);

                        return metricResult;
                    });
                    
                return result;
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
