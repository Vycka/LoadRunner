using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class MinMetric<T> : IMetric<T>
    {
        private readonly LongSelectorDelegate<T> _selector;
        private readonly long _initialValue;
        private long _min;

        public MinMetric(LongSelectorDelegate<T> selector, string name = "Min", long initialValue = long.MaxValue)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));

            _initialValue = initialValue;
            _min = initialValue;

            ColumnNames = new[] { name };
        }


        public IMetric<T> CreateNew()
        {
            return new MinMetric<T>(_selector, ColumnNames[0], _initialValue);
        }

        public void Add(T data)
        {
            long current = _selector(data);
            if (current < _min)
                _min = current;
        }

        public string[] ColumnNames { get; }
        public object[] Values => new object[] { _min };
    }
}