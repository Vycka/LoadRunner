using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics
{
    public class MetricsTemplate<T>
    {
        private readonly IMetric<T>[] _templates;

        public MetricsTemplate(IEnumerable<IMetric<T>> templates)
        {
            if (templates == null)
                throw new ArgumentNullException(nameof(templates));

            _templates = templates.ToArray();
        }

        public IMetric<T> Create()
        {
            return new MetricsMuxer(_templates);
        }

        private class MetricsMuxer : IMetric<T>
        {
            private readonly IMetric<T>[] _metrics;

            public MetricsMuxer(IMetric<T>[] templates)
            {
                _metrics = templates
                    .Select(t => t.CreateNew())
                    .ToArray();
            }

            public IMetric<T> CreateNew()
            {
                return new MetricsMuxer(_metrics);
            }

            public void Add(T data)
            {
                for (int i = 0; i < _metrics.Length; i++)
                {
                    _metrics[i].Add(data);
                }
            }

            string[] IMetric<T>.ColumnNames => _metrics.SelectMany(m => m.ColumnNames).ToArray();

            object[] IMetric<T>.Values => _metrics.SelectMany(m => m.Values).ToArray();
        }
    }
}