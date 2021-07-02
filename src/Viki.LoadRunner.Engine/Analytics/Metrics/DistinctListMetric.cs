using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class DistinctListMetric<TIn, TOut> : IMetric<TIn>
    {
        private readonly string _name;
        private readonly Func<TIn, TOut> _selector;
        private readonly string _separator;

        private readonly HashSet<TOut> _elements;

        public DistinctListMetric(string name, Func<TIn, TOut> selector, string separator = ", ")
        {
            _name = name;
            _selector = selector;
            _separator = separator;

            _elements = new HashSet<TOut>();
        }

        public IMetric<TIn> CreateNew()
        {
            return new DistinctListMetric<TIn, TOut>(_name, _selector, _separator)
            {
                Sort = Sort,
                Limit = Limit
            };
        }

        public void Add(TIn result)
        {
            TOut item = _selector(result);

            if (item != null && _elements.Contains(item) == false)
            {
                _elements.Add(item);
            }
        }

        public Func<IEnumerable<TOut>, IEnumerable<TOut>> Sort { get; set; } = input => input.OrderBy(e => e);

        public int Limit { get; set; } = Int32.MaxValue;

        public string[] ColumnNames => new[] { _name };
        public object[] Values => new object[] { String.Join(_separator, Sort(_elements).Take(Limit)) + (_elements.Count > Limit ? $"... +{_elements.Count - Limit} more items." : "") };
    }
}