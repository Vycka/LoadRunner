using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Dimensions
{
    /// <summary>
    /// Split results in provided time intervals
    /// </summary>
    public class TimeDimension<T> : IDimension<T>
    {
        public readonly TimeSpan Interval;

        private readonly TimeSelectorDelegate<T> _timeSelector;

        public Func<TimeSpan, string> Formatter = t => ((long)t.TotalSeconds).ToString();

        /// <param name="interval">interval timespan</param>
        /// <param name="timeSelector">TimeSpan selector for which dimension key will be calculated</param>
        /// <param name="dimensionName">Custom name for dimension</param>
        public TimeDimension(TimeSpan interval, TimeSelectorDelegate<T> timeSelector, string dimensionName = "Time (s)")
        {
            Interval = interval;
            _timeSelector = timeSelector ?? throw new ArgumentNullException(nameof(timeSelector));
            DimensionName = dimensionName;
        }

        public string DimensionName { get; }

        string IDimension<T>.GetKey(T data)
        {
            TimeSpan resultTimeSlot = Calculate(Interval, _timeSelector(data));

            return Formatter(resultTimeSlot);
        }

        /// <summary>
        /// Calculates TimeSpan value for dimension key.
        /// </summary> 
        public static TimeSpan Calculate(TimeSpan interval, TimeSpan time)
        {
            return TimeSpan.FromTicks(((int)(time.Ticks / interval.Ticks)) * interval.Ticks);
        }

        public delegate TimeSpan TimeSelectorDelegate<in TData>(TData data);
    }
}