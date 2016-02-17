using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    public class MetricMultiplexer : IMetric
    {
        private readonly IMetric[] _metrics;

        public MetricMultiplexer(IEnumerable<IMetric> metricTemplates)
        {
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