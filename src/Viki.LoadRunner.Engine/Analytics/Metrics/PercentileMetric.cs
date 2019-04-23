using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class PercentileMetric<T> : IMetric<T>
    {
        private readonly Func<double, string> _headerSelector;
        private readonly Func<T, double?> _durationSelector;
        private readonly double[] _targetPercentiles;

        private readonly string[] _columnNames;
        private readonly List<double> _inputDurations;

        public Func<double, object> Formatter = (i) => i;

        public PercentileMetric(Func<T, double?> durationSelector, params double[] targetPercentiles)
          : this(targetPercentile => $"p{targetPercentile * 100.0}%", durationSelector, targetPercentiles)
        {
        }

        public PercentileMetric(Func<double, string> headerSelector, Func<T, double?> durationSelector, params double[] percentiles)
        {
            _headerSelector = headerSelector ?? throw new ArgumentNullException(nameof(headerSelector));
            _durationSelector = durationSelector ?? throw new ArgumentNullException(nameof(durationSelector));
            _targetPercentiles = percentiles ?? throw new ArgumentNullException(nameof(percentiles));

            _inputDurations = new List<double>();
            _columnNames = percentiles.Select(headerSelector).ToArray();
        }

        public IMetric<T> CreateNew()
        {
            return new PercentileMetric<T>(_headerSelector, _durationSelector, _targetPercentiles)
            {
                Formatter = Formatter
            };
        }

        public void Add(T data)
        {
            double? duration = _durationSelector(data);
            
            if (duration != null)
                _inputDurations.Add(duration.Value);
        }

        public string[] ColumnNames => _inputDurations.Count > 0 ? _columnNames : Array.Empty<string>();
        public object[] Values => _inputDurations.Count > 0 ? CalculatePercentiles() : Array.Empty<object>();

        private object[] CalculatePercentiles()
        {
            _inputDurations.Sort();

            object[] result = _targetPercentiles
                .Select(p => Formatter(CalculatePercentile(_inputDurations, p)))
                .ToArray();

            return result;
        }

        public static double CalculatePercentile(List<double> sortedData, double percentile)
        {
            double realIndex = percentile * (sortedData.Count - 1);
            int index = (int)realIndex;
            double frac = realIndex - index;
            if (index + 1 < sortedData.Count)
                return sortedData[index] * (1 - frac) + sortedData[index + 1] * frac;
            else
                return sortedData[index];
        }
    }
}