using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    public class MetricMultiplexer : IMetric
    {
        private readonly IMetric[] _metrics;

        /// <summary>
        /// MetricMultiplexer acts as signle metric, but it wraps multiple provided metrics and makes them work as one
        /// </summary>
        /// <param name="metricTemplates"></param>
        public MetricMultiplexer(IEnumerable<IMetric> metricTemplates)
        {
            if (metricTemplates == null)
                throw new ArgumentNullException(nameof(metricTemplates));

            _metrics = metricTemplates.ToArray();
        }


        IMetric IMetric.CreateNew()
        {
            return new MetricMultiplexer(_metrics.Select(m => m.CreateNew()));
        }

        void IMetric.Add(IResult result)
        {
            foreach (IMetric metric in _metrics)
            {
                metric.Add(result);
            }
        }

        string[] IMetric.ColumnNames => _metrics.SelectMany(m => m.ColumnNames).ToArray();
        object[] IMetric.Values => _metrics.SelectMany(m => m.Values).ToArray();
    }
}