using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class SubMetric<TBase, TSub> : IMetric<TBase>
    {
        private readonly SubSelectorDelegate<TSub, TBase> _selector;
        private readonly MetricsTemplate<TSub> _template;
        private readonly IMetric<TSub> _metric;

        public SubMetric(SubSelectorDelegate<TSub, TBase> selector, params IMetric<TSub>[] metrics)
        : this(selector, new MetricsTemplate<TSub>(metrics))
        {
        }

        public SubMetric(SubSelectorDelegate<TSub, TBase> selector, MetricsTemplate<TSub> template)
        {
            _selector = selector;
            _template = template;

            _metric = _template.Create();
        }

        public IMetric<TBase> CreateNew()
        {
            return new SubMetric<TBase, TSub>(_selector, _template);
        }

        public void Add(TBase data)
        {
            IEnumerable<TSub> subInput = _selector(data);
            foreach (TSub subItem in subInput)
            {
                _metric.Add(subItem);
            }
        }

        public string[] ColumnNames => _metric.ColumnNames;
        public object[] Values => _metric.Values;
    }

    public delegate IEnumerable<TSub> SubSelectorDelegate<TSub, TBase>(TBase input);
}