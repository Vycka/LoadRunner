using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class SubMetric<TBase, TSub> : IMetric<TBase>
    {
        private readonly SubSelectorDelegate _selector;
        private readonly IMetric<TSub>[] _metrics;
        private readonly int _metricsCount;

        public SubMetric(SubSelectorDelegate selector, params IMetric<TSub>[] metrics)
        {
            _selector = selector;
            _metrics = metrics.Select(m => m.CreateNew()).ToArray();
            _metricsCount = _metrics.Length;
        }

        public IMetric<TBase> CreateNew()
        {
            return new SubMetric<TBase, TSub>(_selector, _metrics);
        }

        public void Add(TBase data)
        {
            IEnumerable<TSub> subInput = _selector(data);
            foreach (TSub subItem in subInput)
            {
                for (int i = 0; i < _metricsCount; i++)
                {
                    _metrics[i].Add(subItem);
                }
            }
        }

        public string[] ColumnNames => _metrics.SelectMany(m => m.ColumnNames).ToArray();
        public object[] Values => _metrics.SelectMany(m => m.Values).ToArray();

        public delegate IEnumerable<TSub> SubSelectorDelegate(TBase input);
    }
}