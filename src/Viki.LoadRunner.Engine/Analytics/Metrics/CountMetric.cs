using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class CountMetric<T> : IMetric<T>
    {
        private readonly BoolSelectorDelegate<T> _canCountSelector;
        private int _count = 0;

        public CountMetric(string name = "Count")
            : this((i) => true, name)
        {
        }

        public CountMetric(BoolSelectorDelegate<T> canCountSelector, string name = "Count")
        {
            _canCountSelector = canCountSelector;
            ColumnNames = new[] { name };
        }

        public IMetric<T> CreateNew()
        {
            return new CountMetric<T>(_canCountSelector, ColumnNames[0]);
        }

        public void Add(T data)
        {
            if (_canCountSelector(data))
            {
                _count++;
            }
        }

        public string[] ColumnNames { get; }
        public object[] Values => new object[] { _count };
    }
}