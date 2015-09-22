using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// Aggregate results by test execution time
    /// </summary>
    public class TimeHistogramResultsAggregator : IResultsAggregator
    {
        #region Fields

        private readonly TimeSpan _aggregationTimeInterval;
        private DateTime _testBeginTime;

        private readonly IResultsAggregator _histogramResultsAggregator;

        #endregion

        #region Properties

        /// <summary>
        /// Func to retrieve DateTime, on which aggregation is based on
        /// (Default TestContextResult.IterationStarted)
        /// </summary>
        public Func<TestContextResult, DateTime> GetTimeValue = result => result.IterationStarted;

        #endregion

        #region Constructor

        /// <summary>
        /// Groups results by test execution time in provided [aggregationTimeInterval] time periods
        /// </summary>
        /// <param name="aggregationTimeInterval">Time interval to group results by</param>
        public TimeHistogramResultsAggregator(TimeSpan aggregationTimeInterval)
        {
            _aggregationTimeInterval = aggregationTimeInterval;
            _histogramResultsAggregator = new HistogramResultsAggregator(GroupByCalculatorFunction);
        }

        /// <summary>
        /// Groups results by test execution time and secondary aggregation function
        /// </summary>
        /// <param name="aggregationTimeInterval">Time interval to group results by</param>
        /// <param name="subGroupByKeyCalculatorFunction">Secondary aggregation function</param>
        public TimeHistogramResultsAggregator(TimeSpan aggregationTimeInterval, Func<TestContextResult, string> subGroupByKeyCalculatorFunction)
        {
            _aggregationTimeInterval = aggregationTimeInterval;

            Func<TestContextResult, object> groupByFunction =
                result =>
                    string.Concat(GroupByCalculatorFunction(result).ToString(), " ",
                        subGroupByKeyCalculatorFunction(result));

            _histogramResultsAggregator = new HistogramResultsAggregator(groupByFunction);
        }

        #endregion

        #region Group by time function

        private object GroupByCalculatorFunction(TestContextResult result)
        {
            long iterationEndTicks = (GetTimeValue(result) - _testBeginTime).Ticks;

            var resultTimeSlot = ((int) (iterationEndTicks/_aggregationTimeInterval.Ticks)) * _aggregationTimeInterval.Ticks;

            return resultTimeSlot;
        }

        #endregion

        #region IResultsAggregator

        void IResultsAggregator.TestContextResultReceived(TestContextResult result)
        {
            _histogramResultsAggregator.TestContextResultReceived(result);
        }

        void IResultsAggregator.Begin(DateTime testBeginTime)
        {
            _testBeginTime = testBeginTime;
            _histogramResultsAggregator.Begin(testBeginTime);
        }

        void IResultsAggregator.End()
        {
            _histogramResultsAggregator.End();
        }

        #endregion

        #region GetResults()

        /// <summary>
        /// Get Build results object from aggregated data
        /// </summary>
        /// <returns>Aggregated results</returns>
        public IEnumerable<HistogramResultRow> GetResults()
        {
            return ((HistogramResultsAggregator) _histogramResultsAggregator).GetResults();
        }

        #endregion
    }
}