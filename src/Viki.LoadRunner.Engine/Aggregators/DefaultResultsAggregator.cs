using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Viki.LoadRunner.Engine.Aggregators.Aggregates;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public class DefaultResultsAggregator : IResultsAggregator
    {
        private readonly CheckpointOrderLearner _orderLearner = new CheckpointOrderLearner();
        private readonly DefaultTestContextResultAggregate _statsAggregator = new DefaultTestContextResultAggregate();


        public void TestContextResultReceived(TestContextResult result)
        {
            _orderLearner.Learn(result);
            _statsAggregator.AggregateResult(result);
        }

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
    
        public void Begin()
        {
            _statsAggregator.Reset();
            _orderLearner.Reset();
        }

        public void End()
        {

        }
    }
}