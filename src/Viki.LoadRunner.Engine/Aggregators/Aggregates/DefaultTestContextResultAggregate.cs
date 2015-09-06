using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Aggregates
{
    public class DefaultTestContextResultAggregate
    {
        private uint _summedWorkingThreads = 0;
        private uint _summedCreatedThreads = 0;
        private uint _threadAggregationCount = 0;

        public readonly Dictionary<string, DefaultCheckpointAggregate> CheckpointAggregates = new Dictionary<string, DefaultCheckpointAggregate>();
        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public DateTime IterationBeginTime { get; private set; } = DateTime.MaxValue;
        public DateTime IterationEndTime { get; private set; } = DateTime.MinValue;

        public double WorkingThreadsAvg => _summedWorkingThreads / (double) _threadAggregationCount;
        public double CreatedThreadsAvg => _summedCreatedThreads / (double) _threadAggregationCount;

        public void AggregateResult(TestContextResult result)
        {
            MapIterationTime(result);
            AggregateThreadCounts(result);

            Checkpoint previousCheckpoint = PreviousCheckpointBase;
            foreach (Checkpoint currentCheckpoint in result.Checkpoints)
            {
                TimeSpan momentCheckpointTimeSpan = currentCheckpoint.TimePoint - previousCheckpoint.TimePoint;
                DefaultCheckpointAggregate checkpointAggregateResultObject = GetCheckpointAggregate(currentCheckpoint.CheckpointName);

                checkpointAggregateResultObject.AggregateCheckpoint(momentCheckpointTimeSpan, currentCheckpoint);
                previousCheckpoint = currentCheckpoint;
            }
        }

        private void MapIterationTime(TestContextResult result)
        {
            if (result.IterationStarted < IterationBeginTime)
                IterationBeginTime = result.IterationStarted;

            if (result.IterationFinished > IterationEndTime)
                IterationEndTime = result.IterationFinished;
        }

        private void AggregateThreadCounts(TestContextResult result)
        {
            _summedCreatedThreads += (uint) result.CreatedThreads;
            _summedWorkingThreads += (uint) result.WorkingThreads;

            _threadAggregationCount++;
        }

        private DefaultCheckpointAggregate GetCheckpointAggregate(string checkpointName)
        {
            DefaultCheckpointAggregate result;
            if (!CheckpointAggregates.TryGetValue(checkpointName, out result))
            {
                result = new DefaultCheckpointAggregate(checkpointName);
                CheckpointAggregates.Add(checkpointName, result);
            }

            return result;
        }

        public void Reset()
        {
            CheckpointAggregates.Clear();

            _summedCreatedThreads = 0;
            _summedWorkingThreads = 0;
            _threadAggregationCount = 0;

            IterationBeginTime  = DateTime.MaxValue;
            IterationEndTime  = DateTime.MinValue;
        }
    }
}