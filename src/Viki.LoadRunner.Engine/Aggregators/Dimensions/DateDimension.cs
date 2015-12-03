using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Dimensions
{
    /// <summary>
    /// Split results in provided time intervals
    /// </summary>
    public class DateDimension : IDimension
    {
        public readonly TimeSpan Interval;


        /// <param name="interval">interval timespan</param>
        public DateDimension(TimeSpan interval)
        {
            Interval = interval;
        }

        string IDimension.GetKey(TestContextResult result)
        {

            TimeSpan resultTimeSlot = TimeSpan.FromTicks(((int)(result.IterationStarted.Ticks / Interval.Ticks)) * Interval.Ticks);

            return ((long)resultTimeSlot.TotalSeconds).ToString();
        }
    }
}