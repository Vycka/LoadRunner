using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class SumMetric<T> : IMetric<T>
    {
        private readonly Func<T, int> _selector;
        private int _sum;

        public SumMetric(Func<T, int> selector)
            : this("Sum", selector)
        {

        }

        public SumMetric(string name, Func<T, int> selector)
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
}