using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Dimensions
{
    /// <summary>
    /// Split results in provided time intervals
    /// </summary>
    public class TimeDimension : IDimension
    {
        public readonly TimeSpan Interval;


        /// <param name="interval">interval timespan</param>
        public TimeDimension(TimeSpan interval)
        {
            Interval = interval;
        }

        public string DimensionName { get; } = "Time (s)";

        string IDimension.GetKey(TestContextResult result)
        {

            TimeSpan resultTimeSlot = TimeSpan.FromTicks(((int)(result.IterationStarted.Ticks / Interval.Ticks)) * Interval.Ticks);

            return ((long)resultTimeSlot.TotalSeconds).ToString();
        }
    }
}