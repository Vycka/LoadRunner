using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Aggregates;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Results
{
    public class ResultsMapper
    {
        private readonly CheckpointOrderLearner _orderLearner;

        public ResultsMapper(CheckpointOrderLearner orderLearner)
        {
            if (orderLearner == null) throw new ArgumentNullException(nameof(orderLearner));
            _orderLearner = orderLearner;
        }

        public IEnumerable<ResultItemRow> Map(TestContextResultAggregate results, bool includeAllCheckpoints = false)
        {
            IEnumerable<string> resultsOrder = _orderLearner.LearnedOrder;

            List<CheckpointAggregate> orderedResults =
                resultsOrder
                    .Where(results.CheckpointAggregates.ContainsKey)
                    .Select(checkpointName => results.CheckpointAggregates[checkpointName]).ToList();

            if (includeAllCheckpoints)
                yield return
                    new ResultItemRow(results, results.CheckpointAggregates[Checkpoint.IterationSetupCheckpointName]);

            int iterationCount = 0;
            if (results.CheckpointAggregates.ContainsKey(Checkpoint.IterationEndCheckpointName))
            {
                foreach (CheckpointAggregate resultItem in orderedResults.GetRange(2, orderedResults.Count - 3))
                {
                    var resultItemRow = new ResultItemRow(results, resultItem);
                    resultItemRow.SetErrors(orderedResults[1 + iterationCount].Errors);

                    iterationCount++;
                    yield return resultItemRow;
                }
            }
            else if (
                results.CheckpointAggregates.ContainsKey(Checkpoint.IterationStartCheckpointName)
                && results.CheckpointAggregates.ContainsKey(Checkpoint.IterationEndCheckpointName) == false
                )
            {
                foreach (CheckpointAggregate resultItem in orderedResults.GetRange(2, orderedResults.Count - 3))
                {
                    var resultItemRow = new ResultItemRow(results, resultItem);
                    resultItemRow.SetErrors(orderedResults[1 + iterationCount].Errors);

                    iterationCount++;
                    yield return resultItemRow;
                }

                ResultItemRow iterationEndRow = new ResultItemRow(
                    Checkpoint.IterationEndCheckpointName,
                    new ResultItemRow(results, orderedResults[orderedResults.Count - 2])
                )
                {
                    Count = 0,
                    SuccessIterationsPerSec = 0
                };


                yield return iterationEndRow;
            }

            if (includeAllCheckpoints)
                yield return
                    new ResultItemRow(results, results.CheckpointAggregates[Checkpoint.IterationTearDownCheckpointName]);
        }
    }
}