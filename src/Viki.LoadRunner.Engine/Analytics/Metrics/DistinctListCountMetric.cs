using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class DistinctListCountMetric<TIn, TOut> : IMetric<TIn>
    {
        protected readonly string _name;
        protected readonly Func<TIn, TOut> _selector;

        private readonly FlexiRow<TOut, int> _elements;

        public DistinctListCountMetric(string name, Func<TIn, TOut> selector)
        {
            _name = name;
            _selector = selector;

            _elements = new FlexiRow<TOut, int>(() => 0);
        }

        public IMetric<TIn> CreateNew()
        {
            return new DistinctListCountMetric<TIn, TOut>(_name, _selector)
            {
                Sort = Sort
            };
        }

        public void Add(TIn result)
        {
            TOut item = _selector(result);

            if (item != null)
            {
                _elements[item]++;
            }
        }

        public Func<IEnumerable<KeyValuePair<TOut, int>>, IEnumerable<KeyValuePair<TOut, int>>> Sort { get; set; } = input => input.OrderBy(e => e);

        public string[] ColumnNames => new[] { _name };
        public object[] Values => new object[] { String.Join(", ", Sort(_elements).Select(e => String.Concat(e.Key, "[", e.Value, "]"))) };
    }
}