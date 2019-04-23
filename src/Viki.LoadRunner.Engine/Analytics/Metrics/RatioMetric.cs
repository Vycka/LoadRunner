using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Metrics.Calculators;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    /// <summary>
    /// Calculate ratio between of included_count[selector returns true] / total_count
    /// </summary>
    public class RatioMetric<T> : IMetric<T>
    {
        private readonly string _name;
        private readonly Func<T, bool> _selector;
        private readonly double _multiplier;

        private readonly RatioCalculator _calculator = new RatioCalculator();

        public RatioMetric(Func<T, bool> include, double multiplier = 1.0)
            : this("Ratio", include, multiplier)
        {
        }

        public RatioMetric(string name, Func<T, bool> include, double multiplier = 1.0)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _selector = include ?? throw new ArgumentNullException(nameof(include));
            _multiplier = multiplier;
        }

        IMetric<T> IMetric<T>.CreateNew()
        {
            return new RatioMetric<T>(_name, _selector, _multiplier);
        }

        void IMetric<T>.Add(T data)
        {
            _calculator.Add(_selector(data));
        }

        string[] IMetric<T>.ColumnNames => _calculator.TotalCount > 0 ? new[] { _name } : Array.Empty<string>();

        object[] IMetric<T>.Values => _calculator.TotalCount > 0
            ? new object[] {_calculator.Ratio * _multiplier}
            : Array.Empty<object>();
    }
}