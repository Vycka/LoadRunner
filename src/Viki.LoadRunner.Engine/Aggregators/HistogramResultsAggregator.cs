using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Aggregates;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public class HistogramResultsAggregator : IResultsAggregator
    {
        protected Func<TestContextResult, object> _groupByKeyCalculatorFunction;
        
        private readonly CheckpointOrderLearner _orderLearner = new CheckpointOrderLearner();
        private readonly Dictionary<object, TestContextResultAggregate> _histogramItems = new Dictionary<object, TestContextResultAggregate>();

        public IReadOnlyList<string> CheckpointsOorder => _orderLearner.LearnedOrder;

        /// <summary>
        /// Aggregates results, grouping them by [groupByKeyCalculatorFunction]
        /// </summary>
        /// <param name="groupByKeyCalculatorFunction">Function to calculate GroupBy Key</param>
        public HistogramResultsAggregator(Func<TestContextResult, object> groupByKeyCalculatorFunction)
        {
            if (groupByKeyCalculatorFunction == null)
                throw new ArgumentNullException(nameof(groupByKeyCalculatorFunction));

            _groupByKeyCalculatorFunction = groupByKeyCalculatorFunction;
        }

        protected HistogramResultsAggregator()
        {
        }

        public void TestContextResultReceived(TestContextResult result)
        {
            _orderLearner.Learn(result);

            object groupByKey = _groupByKeyCalculatorFunction(result);
            TestContextResultAggregate histogramRowAggregate = GetHistogramRow(groupByKey);
            histogramRowAggregate.AggregateResult(result);
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


        public virtual void Begin(DateTime testBeginTime)
        {
            _histogramItems.Clear();
        }

        public void End()
        {

        }
    }
}