using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class MaxMetric<T> : IMetric<T>
    {
        private readonly LongSelectorDelegate<T> _selector;
        private readonly long _initialValue;
        private long _max;

        public MaxMetric(LongSelectorDelegate<T> selector, string name = "Max", long initialValue = long.MinValue)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));

            _initialValue = initialValue;
            _max = initialValue;

            ColumnNames = new[] { name };
        }


        public IMetric<T> CreateNew()
        {
            return new MaxMetric<T>(_selector, ColumnNames[0], _initialValue);
        }

        public void Add(T data)
        {
            long current = _selector(data);
            if (current > _max)
                _max = current;
        }

        public string[] ColumnNames { get; }
        public object[] Values => new object[] { _max };
    } 
}