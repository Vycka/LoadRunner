using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public class TimeHistogramResultsAggregator : HistogramResultsAggregator
    {
        private readonly TimeSpan _aggregationTimeInterval;
        private DateTime _testBeginTime;

        /// <summary>
        /// Groups results by test execution time in provided [aggregationTimeInterval] time periods
        /// </summary>
        /// <param name="aggregationTimeInterval">Time interval to group results by</param>
        public TimeHistogramResultsAggregator(TimeSpan aggregationTimeInterval)
        {
            _aggregationTimeInterval = aggregationTimeInterval;
            _groupByKeyCalculatorFunction = GroupByCalculatorFunction;
        }

        public override void Begin(DateTime testBeginTime)
        {
            _testBeginTime = testBeginTime;

            base.Begin(testBeginTime);
        }

        private object GroupByCalculatorFunction(TestContextResult result)
        {
            long iterationEndTicks = (result.IterationFinished - _testBeginTime).Ticks;

            var resultTimeSlot = ((int) (iterationEndTicks/_aggregationTimeInterval.Ticks)) * _aggregationTimeInterval.Ticks;

            return resultTimeSlot;
        }
    }
}