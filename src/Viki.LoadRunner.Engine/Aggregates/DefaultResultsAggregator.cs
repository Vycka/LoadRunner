using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregates.Results;
using Viki.LoadRunner.Engine.Aggregates.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates
{
    public class DefaultResultsAggregator : IResultsAggregator
    {
        private readonly CheckpointOrderLearner _orderLearner = new CheckpointOrderLearner();
        private readonly Dictionary<string, ResultItemRow> _results = new Dictionary<string, ResultItemRow>();


        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public void TestContextResultReceived(TestContextResult resultItem)
        {
            _orderLearner.Learn(resultItem);

            Checkpoint previousCheckpoint = PreviousCheckpointBase;
            foreach (Checkpoint currentCheckpoint in resultItem.Checkpoints)
            {
                TimeSpan momentCheckpointTimeSpan = currentCheckpoint.TimePoint - previousCheckpoint.TimePoint;
                ResultItemRow checkpointResultObject = GetCheckpointResultObject(currentCheckpoint.CheckpointName);

                checkpointResultObject.AggregateResult(momentCheckpointTimeSpan, currentCheckpoint, resultItem);
                previousCheckpoint = currentCheckpoint;
            }
        }

        public void Reset()
        {
            _orderLearner.Reset();
            _results.Clear();
        }

        private ResultItemRow GetCheckpointResultObject(string checkpointName)
        {
            ResultItemRow result = null;

            if (_results.ContainsKey(checkpointName))
            {
                result = _results[checkpointName];
            }
            else
            {
                result = new ResultItemRow(checkpointName);
                _results.Add(checkpointName, result); 
            }

            return result;
        }
        public IEnumerable<ResultItem> GetResults()
        {
            List<ResultItemRow> orderedResults =
                _orderLearner.LearnedOrder.Select(checkpointName => _results[checkpointName]).ToList();

            foreach (ResultItemRow resultItem in orderedResults.GetRange(1, orderedResults.Count - 2))
            {
                yield return resultItem;
            }

            ResultItemTotals resultItemTotals = new ResultItemTotals(_results);
            yield return resultItemTotals;
        }
    }
}