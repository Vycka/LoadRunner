using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Aggregates;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// Aggregates results by provided group key using Func&lt;TestContextResult, object&gt;()
    /// </summary>
    public class HistogramResultsAggregator : IResultsAggregator
    {
        #region Fields

        private readonly Func<TestContextResult, object> _groupByKeyCalculatorFunction;

        private readonly CheckpointOrderLearner _orderLearner = new CheckpointOrderLearner();

        private readonly Dictionary<object, TestContextResultAggregate> _histogramItems =
            new Dictionary<object, TestContextResultAggregate>();

        #endregion

        #region Properties

        /// <summary>
        /// Learned order of checkpoints
        /// </summary>
        public IReadOnlyList<string> CheckpointsOorder => _orderLearner.LearnedOrder;

        #endregion

        #region Constructor

        /// <summary>
        /// Aggregates results, grouping them by [groupByKeyCalculatorFunction]
        /// </summary>
        /// <param name="groupByKeyCalculatorFunction">Function to calculate GroupBy key</param>
        public HistogramResultsAggregator(Func<TestContextResult, object> groupByKeyCalculatorFunction)
        {
            if (groupByKeyCalculatorFunction == null)
                throw new ArgumentNullException(nameof(groupByKeyCalculatorFunction));

            _groupByKeyCalculatorFunction = groupByKeyCalculatorFunction;
        }

        #endregion

        #region IResultsAggregator

        void IResultsAggregator.TestContextResultReceived(TestContextResult result)
        {
            _orderLearner.Learn(result);

            object groupByKey = _groupByKeyCalculatorFunction(result);
            TestContextResultAggregate histogramRowAggregate = GetHistogramRow(groupByKey);
            histogramRowAggregate.AggregateResult(result);
        }

        void IResultsAggregator.Begin(DateTime testBeginTime)
        {
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
        /// Get Build results object from aggregated data
        /// </summary>
        /// <returns>Aggregated results</returns>
        public IEnumerable<HistogramResultRow> GetResults()
        {
            if (_orderLearner.LearnedOrder.Count != 0)
            {
                ResultsMapper mapper = new ResultsMapper(_orderLearner);
                foreach (KeyValuePair<object, TestContextResultAggregate> histogramItem in _histogramItems)
                {
                    HistogramResultRow result = new HistogramResultRow(
                        histogramItem.Key,
                        histogramItem.Value,
                        mapper.Map(histogramItem.Value, true).ToList()
                        );

                    yield return result;
                }
            }
        }

        #endregion
    }
}