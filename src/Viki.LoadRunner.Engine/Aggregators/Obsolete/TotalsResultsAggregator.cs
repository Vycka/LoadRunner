using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Aggregates;
using Viki.LoadRunner.Engine.Aggregators.Obsolete.Results;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Obsolete
{
    /// <summary>
    /// Calculates totals without doing any aggregations
    /// </summary>
    [Obsolete("Use HistogramAggregator")]
    public class TotalsResultsAggregator : IResultsAggregator
    {
        #region Fields

        private readonly OrderLearner _orderLearner = new OrderLearner();
        private readonly TestContextResultAggregate _statsAggregator = new TestContextResultAggregate();

        #endregion

        #region IResultsAggregator

        void IResultsAggregator.TestContextResultReceived(IResult result)
        {
            _orderLearner.Learn(result.Checkpoints.Select(c => c.Name).ToArray());
            _statsAggregator.AggregateResult(result);
        }

        void IResultsAggregator.Begin()
        {
            _statsAggregator.Reset();
            _orderLearner.Reset();
        }

        void IResultsAggregator.End()
        {
        }

        #endregion

        #region GetResults()

        /// <summary>
        /// Get Build results object from aggregated data
        /// </summary>
        /// <returns>Aggregated results</returns>
        public ResultsContainer GetResults()
        {
            ResultsContainer result = null;
            if (_orderLearner.LearnedOrder.Count != 0)
            {
                ResultsMapper mapper = new ResultsMapper(_orderLearner);
                IEnumerable<ResultItemRow> resultRows = mapper.Map(_statsAggregator);
                result = new ResultsContainer(resultRows.ToList(), new ResultItemTotals(_statsAggregator));
            }
            return result;
        }

        #endregion
    }
}