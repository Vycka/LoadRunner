using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates.Aggregates
{
    public class DefaultTestContextResultAggregate
    {
        public readonly Dictionary<string, DefaultCheckpointAggregate> CheckpointAggregates = new Dictionary<string, DefaultCheckpointAggregate>();
        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);

        public DateTime IterationBeginTime { get; private set; } = DateTime.MaxValue;
        public DateTime IterationEndTime { get; private set; } = DateTime.MinValue;

        public void AggregateResult(TestContextResult result)
        {
            MapIterationTime(result);

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
        }
    }
}