using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// Aggregate results by test execution time
    /// </summary>
    [Obsolete("Use HistogramAggregator")]
    public class TimeHistogramResultsAggregator : IResultsAggregator
    {
        #region Fields

        private readonly HistogramResultsAggregator _histogramResultsAggregator;

        #endregion

        #region Properties

        /// <summary>
        /// Func to retrieve DateTime, on which aggregation is based on
        /// (Default TestContextResult.IterationStarted)
        /// </summary>
        public Func<IterationResult, TimeSpan> GetTimeValue = result => result.IterationStarted;

        #endregion

        #region Constructor

        /// <summary>
        /// Groups results by test execution time in provided [aggregationTimeInterval] time periods
        /// </summary>
        /// <param name="aggregationTimeInterval">Time interval to group results by</param>
        public TimeHistogramResultsAggregator(TimeSpan aggregationTimeInterval)
        {
            _histogramResultsAggregator = new HistogramResultsAggregator(result => GroupByCalculatorFunction(result))
            {
                AggregationTimePeriod = aggregationTimeInterval
            };
        }

        /// <summary>
        /// Groups results by test execution time and secondary aggregation function
        /// </summary>
        /// <param name="aggregationTimeInterval">Time interval to group results by</param>
        /// <param name="subGroupByKeyCalculatorFunction">Secondary aggregation function</param>
        public TimeHistogramResultsAggregator(TimeSpan aggregationTimeInterval, Func<IResult, string> subGroupByKeyCalculatorFunction)
        {
            Func<IResult, object> groupByFunction =
                result =>
                    string.Concat(GroupByCalculatorFunction(result).ToString(), " ", subGroupByKeyCalculatorFunction(result));

            _histogramResultsAggregator = new HistogramResultsAggregator(groupByFunction)
            {
                AggregationTimePeriod = aggregationTimeInterval
            };
        }

        /// <summary>
        /// Groups results by provided aggregation function (which will receive key used by TimeAggregation)
        /// And it gives full ability to postprocess GroupByKey before returning it.
        /// </summary>
        /// <param name="aggregationTimeInterval">Time interval to group results by</param>
        /// <param name="groupByKeyCalculatorFunction">GroupBy function Func&lt;TestContextResult, TimeAggregationSlotTicks, GroupByKey&gt;</param>
        public TimeHistogramResultsAggregator(TimeSpan aggregationTimeInterval, Func<IResult, long, object> groupByKeyCalculatorFunction)
        {
            Func<IResult, object> groupByFunction = result => groupByKeyCalculatorFunction(result, GroupByCalculatorFunction(result));

            _histogramResultsAggregator = new HistogramResultsAggregator(groupByFunction)
            {
                AggregationTimePeriod = aggregationTimeInterval
            };
        }

        #endregion

        #region Group by time function

        private long GroupByCalculatorFunction(IResult result)
        {

            var resultTimeSlot =
                // ReSharper disable once PossibleInvalidOperationException
                ((int) (result.IterationStarted.Ticks / _histogramResultsAggregator.AggregationTimePeriod.Value.Ticks))
                * _histogramResultsAggregator.AggregationTimePeriod.Value.Ticks;

            return resultTimeSlot;
        }

        #endregion

        #region IResultsAggregator

        void IResultsAggregator.TestContextResultReceived(IResult result)
        {
            ((IResultsAggregator)_histogramResultsAggregator).TestContextResultReceived(result);
        }

        void IResultsAggregator.Begin()
        {
            ((IResultsAggregator)_histogramResultsAggregator).Begin();
        }

        void IResultsAggregator.End()
        {
            ((IResultsAggregator)_histogramResultsAggregator).End();
        }

        #endregion

        #region GetResults()

        /// <summary>
        /// Get Build results object from aggregated data
        /// </summary>
        /// <returns>Aggregated results</returns>
        public IEnumerable<HistogramResultRow> GetResults()
        {
            return _histogramResultsAggregator.GetResults();
        }

        #endregion
    }
}