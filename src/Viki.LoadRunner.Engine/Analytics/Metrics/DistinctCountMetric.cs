using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class DistinctCountMetric<TIn, TVal> : IMetric<TIn>
    {
        protected readonly string _name;
        protected readonly Func<TIn, TVal> _selector;

        private readonly HashSet<TVal> _elements;

        public DistinctCountMetric(Func<TIn, TVal> selector, string name = "Distinct (Count)")
        {
            _name = name;
            _selector = selector;

            _elements = new HashSet<TVal>();
        }

        public IMetric<TIn> CreateNew()
        {
            return new DistinctCountMetric<TIn, TVal>(_selector, _name);
        }

        public void Add(TIn result)
        {
            TVal item = _selector(result);

            if (item != null && _elements.Contains(item) == false)
            {
                _elements.Add(item);
            }
        }

        public string[] ColumnNames => new[] { _name };
        public object[] Values => new object[] { _elements.Count };
    }
}