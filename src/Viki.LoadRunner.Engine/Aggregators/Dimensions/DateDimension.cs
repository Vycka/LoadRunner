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
        private DateTime _testBeginTime;


        /// <param name="interval">interval timespan</param>
        public DateDimension(TimeSpan interval)
        {
            Interval = interval;
        }

        string IDimension.GetKey(TestContextResult result)
        {
            long timePointTicks = (result.IterationStarted - _testBeginTime).Ticks;

            TimeSpan resultTimeSlot = TimeSpan.FromTicks(((int)(timePointTicks / Interval.Ticks)) * Interval.Ticks);

            return ((long)resultTimeSlot.TotalSeconds).ToString();
        }

        void IDimension.SetBegin(DateTime testBeginTime)
        {
            _testBeginTime = testBeginTime;
        }
    }
}