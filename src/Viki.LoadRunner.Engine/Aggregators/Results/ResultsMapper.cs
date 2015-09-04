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

        public IEnumerable<ResultItemRow> Map(DefaultTestContextResultAggregate results, bool includeAllCheckpoints = false)
        {
            if (results.CheckpointAggregates.Count > 2)
            {
                List<DefaultCheckpointAggregate> orderedResults =
                    _orderLearner.LearnedOrder
                        .Where(results.CheckpointAggregates.ContainsKey)
                        .Select(checkpointName => results.CheckpointAggregates[checkpointName]).ToList();

                if (includeAllCheckpoints)
                    yield return
                        new ResultItemRow(results, results.CheckpointAggregates[Checkpoint.IterationSetupCheckpointName]);

                int iterationCount = 0;
                foreach (DefaultCheckpointAggregate resultItem in orderedResults.GetRange(2, orderedResults.Count - 3))
                {
                    var resultItemRow = new ResultItemRow(results, resultItem);
                    resultItemRow.SetErrors(orderedResults[1 + iterationCount].Errors);

                    iterationCount++;
                    yield return resultItemRow;
                }

                if (includeAllCheckpoints)
                    yield return
                        new ResultItemRow(results, results.CheckpointAggregates[Checkpoint.IterationTearDownCheckpointName]);
            }
            else
            {
                int x = 0;
            }
        }
    }
}