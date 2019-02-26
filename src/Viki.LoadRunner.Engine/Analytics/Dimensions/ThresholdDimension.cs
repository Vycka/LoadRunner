using System;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Dimensions
{
    public class ThresholdDimension<TData, TDuration> : IDimension<TData>
        where TDuration : IComparable<TDuration>
    {
        private readonly ValueSelector _selector;

        private readonly TDuration[] _thresholds;
        private readonly string[] _thresholdStrings;

        public string DefaultOutOfRange = "OutOfRange";

        public ThresholdDimension(ValueSelector selector, TDuration[] thresholds)
            : this(selector, value => value.ToString(), thresholds)
        {
        }

        public ThresholdDimension(ValueSelector selector, DimensionFormatter formatter, params TDuration[] thresholds)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _thresholds = thresholds ?? throw new ArgumentNullException(nameof(thresholds));

            Array.Sort(_thresholds);
            _thresholdStrings = _thresholds.Select(t => formatter(t)).ToArray();
        }

        public string GetKey(TData data)
        {
            TDuration value = _selector(data);

            for (int i = 0; i < _thresholds.Length; i++)
            {
                if (value.CompareTo(_thresholds[i]) == -1)
                    return _thresholdStrings[i];
            }

            return DefaultOutOfRange;
        }

        public string DimensionName { get; set; } = "Threshold";

        public delegate TDuration ValueSelector(TData data);
        public delegate string DimensionFormatter(TDuration value);
    }
}