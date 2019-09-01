using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Metrics.Calculators;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class AverageMetric<T> : IMetric<T>
    {
        private readonly string _name;
        private readonly DoubleSelectorDelegate<T> _selector;

        private readonly AverageCalculator _calculator = new AverageCalculator();

        public AverageMetric(DoubleSelectorDelegate<T> selector)
            : this("Average", selector)
        {
        }

        public AverageMetric(string name, DoubleSelectorDelegate<T> selector)
        {
            _name = name;
            _selector = selector;
        }

        IMetric<T> IMetric<T>.CreateNew()
        {
            return new AverageMetric<T>(_name, _selector);
        }

        void IMetric<T>.Add(T result)
        {
            _calculator.Add(_selector(result));
        }

        string[] IMetric<T>.ColumnNames => _calculator.SampleCount > 0 ? new[] { _name } : Array.Empty<string>();

        object[] IMetric<T>.Values => _calculator.SampleCount > 0
            ? new object[] { _calculator.GetAverage() }
            : Array.Empty<object>();
    }
}