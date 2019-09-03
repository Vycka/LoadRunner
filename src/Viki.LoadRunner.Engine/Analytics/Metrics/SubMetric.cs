using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class SubMetric<TBase, TSub> : IMetric<TBase>
    {
        private readonly SubSelectorDelegate<TSub, TBase> _selector;
        private readonly BoolSelectorDelegate<TBase, TSub> _filter;
        private readonly MetricsTemplate<TSub> _template;
        private readonly IMetric<TSub> _metric;



        public SubMetric(SubSelectorDelegate<TSub, TBase> selector, params IMetric<TSub>[] metrics)
        : this(selector, (data, sub) => true, new MetricsTemplate<TSub>(metrics))
        {
        }

        public SubMetric(SubSelectorDelegate<TSub, TBase> selector, BoolSelectorDelegate<TBase, TSub> filter, params IMetric<TSub>[] metrics)
            : this(selector, filter, new MetricsTemplate<TSub>(metrics))
        {
        }

        public SubMetric(SubSelectorDelegate<TSub, TBase> selector, BoolSelectorDelegate<TBase, TSub> filter, MetricsTemplate<TSub> template)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _template = template ?? throw new ArgumentNullException(nameof(template));

            _metric = _template.Create();
        }

        public IMetric<TBase> CreateNew()
        {
            return new SubMetric<TBase, TSub>(_selector, _filter, _template);
        }

        public void Add(TBase data)
        {
            IEnumerable<TSub> subInput = _selector(data).Where(s => _filter(data, s));
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