using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregator
{
    public class ResultsAggregator
    {
        private ConcurrentQueue<TestContextResult> _processingQueue = new ConcurrentQueue<TestContextResult>();

        private readonly List<string> _checkpointsOrder = new List<string>();
        private readonly Dictionary<string, ResultItem> _results = new Dictionary<string, ResultItem>();

        public void AddResult(TestContextResult result)
        {
            _processingQueue.Enqueue(result);
        }

        public void Reset()
        {
            _processingQueue = new ConcurrentQueue<TestContextResult>();
            _checkpointsOrder.Clear();
            _results.Clear();
        }

        public void ProcessResults()
        {

            TestContextResult resultItem;
            while (_processingQueue.TryDequeue(out resultItem))
            {
                if (resultItem.Exceptions.Length != 0)
                {
                    ResultItem resultObject = GetCheckpointResultObject(Checkpoint.ErrorsCheckpointName, null);
                    resultObject.AddTimeMeassure(TimeSpan.Zero);
                }

                Checkpoint previousCheckpoint = null;
                foreach (Checkpoint checkpoint in resultItem.Checkpoints)
                {
                    ResultItem resultObject = GetCheckpointResultObject(checkpoint.CheckpointName, previousCheckpoint);

                    TimeSpan checkpointTimespan = 
                        previousCheckpoint != null
                        ? checkpoint.TimePoint - previousCheckpoint.TimePoint
                        : checkpoint.TimePoint;

                    resultObject.AddTimeMeassure(checkpointTimespan);

                    previousCheckpoint = checkpoint;
                }

                if (previousCheckpoint?.CheckpointName == Checkpoint.IterationEndCheckpointName)
                {
                    ResultItem totalsResultObject = GetCheckpointResultObject(Checkpoint.TotalsCheckpointName, previousCheckpoint);
                    totalsResultObject.AddTimeMeassure(previousCheckpoint.TimePoint);
                }
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

                if (previousCheckpoint == null)
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