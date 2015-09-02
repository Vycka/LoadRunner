using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates.Default
{
    public class DefaultResultsAggregator : IResultsAggregator
    {
        private readonly List<string> _checkpointsOrder = new List<string>();
        private readonly Dictionary<string, ResultItem> _results = new Dictionary<string, ResultItem>();

        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public void TestContextResultReceived(TestContextResult resultItem)
        {
            Checkpoint previousCheckpoint = PreviousCheckpointBase;
            foreach (Checkpoint currentCheckpoint in resultItem.Checkpoints)
            {
                TimeSpan momentCheckpointTimeSpan = currentCheckpoint.TimePoint - previousCheckpoint.TimePoint;
                ResultItem checkpointResultObject = GetCheckpointResultObject(currentCheckpoint.CheckpointName, previousCheckpoint);

                checkpointResultObject.AggregateResult(momentCheckpointTimeSpan, currentCheckpoint, resultItem);
                previousCheckpoint = currentCheckpoint;
            }
        }

        public List<ResultItem> BuildResultsObject()
        {
            return _checkpointsOrder.Select(checkpointName => _results[checkpointName]).ToList();
        }

        private ResultItem GetCheckpointResultObject(string checkpointName, Checkpoint previousCheckpoint)
        {
            ResultItem result = null;

            if (_results.ContainsKey(checkpointName))
            {
                result = _results[checkpointName];
            }
            else
            {
                result = new ResultItem(checkpointName);
                _results.Add(checkpointName, result);

                if (_checkpointsOrder.Count == 0)
                {
                    _checkpointsOrder.Add(checkpointName);
                }
                else
                {
                    int insertPosition = _checkpointsOrder.FindIndex(s => s == previousCheckpoint.CheckpointName) + 1;

                    if (insertPosition == 0 || insertPosition == _checkpointsOrder.Count)
                        _checkpointsOrder.Add(checkpointName);
                    else
                        _checkpointsOrder.Insert(insertPosition, checkpointName);
                }
            }

            return result;
        }
    }
}