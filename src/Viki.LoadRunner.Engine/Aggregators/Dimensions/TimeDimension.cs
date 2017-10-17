using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Dimensions
{
    /// <summary>
    /// Split results in provided time intervals
    /// </summary>
    public class TimeDimension : IDimension
    {
        public readonly TimeSpan Interval;

        public Func<IResult, TimeSpan> TimeSelector = r => r.IterationFinished;
        public Func<TimeSpan, string> Formatter = t => ((long) t.TotalSeconds).ToString();

        /// <param name="interval">interval timespan</param>
        public TimeDimension(TimeSpan interval, string dimensionName = "Time (s)")
        {
            Interval = interval;
            DimensionName = dimensionName;
        }

        public string DimensionName { get; }
   
        string IDimension.GetKey(IResult result)
        {
            TimeSpan resultTimeSlot = Calculate(Interval, TimeSelector(result));

            return Formatter(resultTimeSlot);
        }

        /// <summary>
        /// Calculates TimeSpan value for dimension key.
        /// </summary> 
        public static TimeSpan Calculate(TimeSpan interval, TimeSpan time)
        {
            return TimeSpan.FromTicks(((int) (time.Ticks / interval.Ticks)) * interval.Ticks);
        }
    }
}