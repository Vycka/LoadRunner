using System;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

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

        string IDimension.GetKey(IResult result)
        {
            TimeSpan resultTimeSlot = TimeSpan.FromTicks(((int)(result.IterationFinished.Ticks / Interval.Ticks)) * Interval.Ticks);

            return ((long)resultTimeSlot.TotalSeconds).ToString();
        }
    }
}