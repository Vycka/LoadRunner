using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregates.Utils;

namespace Viki.LoadRunner.Engine.Aggregates.Results
{
    public class ResultsMapper
    {
        private readonly CheckpointOrderLearner _orderLearner;

        public ResultsMapper(CheckpointOrderLearner orderLearner)
        {
            if (orderLearner == null) throw new ArgumentNullException(nameof(orderLearner));
            _orderLearner = orderLearner;
        }

        public IEnumerable<ResultItemRow> Map(Dictionary<string, AggregatedCheckpoint> results)
        {
            List<AggregatedCheckpoint> orderedResults =
                _orderLearner.LearnedOrder
                .Where(results.ContainsKey)
                .Select(checkpointName => results[checkpointName]).ToList();

            int iterationCount = 0;
            foreach (AggregatedCheckpoint resultItem in orderedResults.GetRange(2, orderedResults.Count - 3))
            {
                var resultItemRow = new ResultItemRow(resultItem);
                resultItemRow.SetErrors(orderedResults[1 + iterationCount].Errors);

                iterationCount++;
                yield return resultItemRow;
            }
        }
    }
}