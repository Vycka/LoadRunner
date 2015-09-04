using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregates.Aggregates;
using Viki.LoadRunner.Engine.Aggregates.Results;
using Viki.LoadRunner.Engine.Aggregates.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates
{
    public class DefaultResultsAggregator : IResultsAggregator
    {
        private readonly CheckpointOrderLearner _orderLearner = new CheckpointOrderLearner();
        private readonly Dictionary<string, CheckpointAggregate> _results = new Dictionary<string, CheckpointAggregate>();


        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public void TestContextResultReceived(TestContextResult resultItem)
        {
            _orderLearner.Learn(resultItem);

            Checkpoint previousCheckpoint = PreviousCheckpointBase;
            foreach (Checkpoint currentCheckpoint in resultItem.Checkpoints)
            {
                TimeSpan momentCheckpointTimeSpan = currentCheckpoint.TimePoint - previousCheckpoint.TimePoint;
                CheckpointAggregate checkpointAggregateResultObject = GetCheckpointResultObject(currentCheckpoint.CheckpointName);

                checkpointAggregateResultObject.AggregateCheckpoint(momentCheckpointTimeSpan, currentCheckpoint, resultItem);
                previousCheckpoint = currentCheckpoint;
            }
        }
        
        public void Reset()
        {
            _orderLearner.Reset();
            _results.Clear();
        }

        private CheckpointAggregate GetCheckpointResultObject(string checkpointName)
        {
            CheckpointAggregate result = null;

            if (_results.ContainsKey(checkpointName))
            {
                result = _results[checkpointName];
            }
            else
            {
                result = new CheckpointAggregate(checkpointName);
                _results.Add(checkpointName, result); 
            }

            return result;
        }
        public ResultsContainer GetResults()
        {
            ResultsMapper mapper = new ResultsMapper(_orderLearner);
            IEnumerable<ResultItemRow> resultRows = mapper.Map(_results);
            return new ResultsContainer(resultRows.ToList(), new ResultItemTotals(_results));
        }
    }
}