using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class SumMetric<T> : IMetric<T>
    {
        private readonly LongSelectorDelegate<T> _selector;
        private long _sum;

        public SumMetric(LongSelectorDelegate<T> selector)
            : this("Sum", selector)
        {

        }

        public SumMetric(string name, LongSelectorDelegate<T> selector)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _selector = selector ?? throw new ArgumentNullException(nameof(selector));

            ColumnNames = new[] { name };
            _sum = 0;
        }

        public IMetric<T> CreateNew()
        {
            return new SumMetric<T>(ColumnNames[0], _selector);
        }

        public void Add(T data)
        {
            _sum += _selector(data);
        }

        public string[] ColumnNames { get; }
        public object[] Values => new object[] { _sum };
    }

    public class SumMetricD<T> : IMetric<T>
    {
        private readonly DoubleSelectorDelegate<T> _selector;
        private double _sum;

        public SumMetricD(DoubleSelectorDelegate<T> selector)
            : this("Sum", selector)
        {

        }

        public SumMetricD(string name, DoubleSelectorDelegate<T> selector)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _selector = selector ?? throw new ArgumentNullException(nameof(selector));

            ColumnNames = new[] { name };
            _sum = 0;
        }

        public IMetric<T> CreateNew()
        {
            return new SumMetricD<T>(ColumnNames[0], _selector);
        }

        public void Add(T data)
        {
            _sum += _selector(data);
        }

        public string[] ColumnNames { get; }
        public object[] Values => new object[] { _sum };
    }
}