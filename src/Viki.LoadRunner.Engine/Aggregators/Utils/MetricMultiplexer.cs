using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    internal class MetricMultiplexer : IMetric
    {
        private readonly IMetric[] _metrics;

        public MetricMultiplexer(IEnumerable<IMetric> metricTemplates)
        {
            _metrics = metricTemplates.ToArray();
        }


        public IMetric CreateNew()
        {
            return new MetricMultiplexer(_metrics.Select(m => m.CreateNew()));
        }

        public void Add(IResult result)
        {
            foreach (IMetric metric in _metrics)
            {
                metric.Add(result);
            }
        }

        public string[] ColumnNames => _metrics.SelectMany(m => m.ColumnNames).ToArray();
        public object[] Values => _metrics.SelectMany(m => m.Values).ToArray();
    }
}