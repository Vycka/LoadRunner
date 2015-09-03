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
        private readonly Dictionary<string, AggregatedCheckpoint> _results = new Dictionary<string, AggregatedCheckpoint>();


        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public void TestContextResultReceived(TestContextResult resultItem)
        {
            _orderLearner.Learn(resultItem);

            Checkpoint previousCheckpoint = PreviousCheckpointBase;
            foreach (Checkpoint currentCheckpoint in resultItem.Checkpoints)
            {
                TimeSpan momentCheckpointTimeSpan = currentCheckpoint.TimePoint - previousCheckpoint.TimePoint;
                AggregatedCheckpoint checkpointResultObject = GetCheckpointResultObject(currentCheckpoint.CheckpointName);

                checkpointResultObject.AggregateCheckpoint(momentCheckpointTimeSpan, currentCheckpoint, resultItem);
                previousCheckpoint = currentCheckpoint;
            }
        }

        public void Reset()
        {
            _orderLearner.Reset();
            _results.Clear();
        }

        private AggregatedCheckpoint GetCheckpointResultObject(string checkpointName)
        {
            AggregatedCheckpoint result = null;

            if (_results.ContainsKey(checkpointName))
            {
                result = _results[checkpointName];
            }
            else
            {
                result = new AggregatedCheckpoint(checkpointName);
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