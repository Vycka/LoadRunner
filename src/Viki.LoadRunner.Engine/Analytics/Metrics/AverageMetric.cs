using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Analytics.Metrics.Calculators;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class AverageMetric<T> : IMetric<T>
    {
        private readonly string _name;
        private readonly Func<T, double> _selector;

        private readonly AverageCalculator _calculator = new AverageCalculator();

        public AverageMetric(Func<T, double> selector)
            : this("Average", selector)
        {
        }

        public AverageMetric(string name, Func<T, double> selector)
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

        string[] IMetric<T>.ColumnNames => new[] { _name };
        object[] IMetric<T>.Values => new object[] { _calculator.GetAverage() };
    }
}