using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Aggregates;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// Aggregates results by provided group key using Func&lt;TestContextResult, object&gt;()
    /// </summary>
    public class HistogramResultsAggregator : IResultsAggregator
    {
        #region Fields

        // TODO: HistogramResultsAggregator shouldn't know anything about timing, move it up!
        private DateTime _testBeginTime;

        private readonly Func<IResult, object> _groupByKeyCalculatorFunction;
        private readonly OrderLearner _orderLearner = new OrderLearner();
        private readonly Dictionary<object, TestContextResultAggregate> _histogramItems =
            new Dictionary<object, TestContextResultAggregate>();

        #endregion

        #region Properties

        /// <summary>
        /// Learned order of checkpoints
        /// </summary>
        public IReadOnlyList<string> CheckpointsOorder => _orderLearner.LearnedOrder;

        /// <summary>
        /// If concrete Aggregation time is known, specifying it here will gives more accurate TPS result value.
        /// Otherwise it will be predicted by raw TestResult data
        /// </summary>
        public TimeSpan? AggregationTimePeriod = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Aggregates results, grouping them by [groupByKeyCalculatorFunction]
        /// </summary>
        /// <param name="groupByKeyCalculatorFunction">Function to calculate GroupBy key</param>
        public HistogramResultsAggregator(Func<IResult, object> groupByKeyCalculatorFunction)
        {
            if (groupByKeyCalculatorFunction == null)
                throw new ArgumentNullException(nameof(groupByKeyCalculatorFunction));

            _groupByKeyCalculatorFunction = groupByKeyCalculatorFunction;
        }

        #endregion

        #region IResultsAggregator

        void IResultsAggregator.TestContextResultReceived(IResult result)
        {
            _orderLearner.Learn(result.Checkpoints.Select(c => c.Name).ToArray());

            object groupByKey = _groupByKeyCalculatorFunction(result);
            TestContextResultAggregate histogramRowAggregate = GetHistogramRow(groupByKey);
            histogramRowAggregate.AggregateResult(result);
        }

        void IResultsAggregator.Begin(DateTime testBeginTime)
        {
            _testBeginTime = testBeginTime;
            _histogramItems.Clear();
        }

        void IResultsAggregator.End()
        {
        }

        private TestContextResultAggregate GetHistogramRow(object aggregateSlot)
        {
            TestContextResultAggregate result;
            if (!_histogramItems.TryGetValue(aggregateSlot, out result))
            {
                result = new TestContextResultAggregate();
                _histogramItems.Add(aggregateSlot, result);
            }

            return result;
        }

        #endregion

        #region Build results

        /// <summary>
        /// Build results object from aggregated data
        /// </summary>
        /// <returns>Aggregated results</returns>
        public IEnumerable<HistogramResultRow> GetResults()
        {
            if (_orderLearner.LearnedOrder.Count != 0)
            {
                TimeSpan lastAggregationBeginMark = TimeSpan.MinValue;

                if (AggregationTimePeriod != null)
                {
                    TimeSpan lastIterationBeginMark = _histogramItems.Max(h => h.Value.IterationBeginTime);

                    lastAggregationBeginMark = TimeSpan.FromTicks(
                        (lastIterationBeginMark.Ticks / AggregationTimePeriod.Value.Ticks)
                        * AggregationTimePeriod.Value.Ticks
                    );
                }

                ResultsMapper mapper = new ResultsMapper(_orderLearner);
                foreach (KeyValuePair<object, TestContextResultAggregate> histogramItem in _histogramItems)
                {
                    HistogramResultRow result = new HistogramResultRow(
                        histogramItem.Key,
                        histogramItem.Value,
                        mapper.Map(histogramItem.Value, true, histogramItem.Value.IterationBeginTime >= lastAggregationBeginMark ? null : AggregationTimePeriod).ToList()
                        );

                    yield return result;
                }
            }
        }

        #endregion
    }
}