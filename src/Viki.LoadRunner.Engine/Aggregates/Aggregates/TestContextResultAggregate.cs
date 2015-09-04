using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates.Aggregates
{
    public class TestContextResultAggregate
    {
        public readonly Dictionary<string, CheckpointAggregate> CheckpointAggregates = new Dictionary<string, CheckpointAggregate>();

        private readonly static Checkpoint PreviousCheckpointBase = new Checkpoint("", TimeSpan.Zero);



        public void AggregateResult(TestContextResult result)
        {
            Checkpoint previousCheckpoint = PreviousCheckpointBase;
            foreach (Checkpoint currentCheckpoint in result.Checkpoints)
            {
                TimeSpan momentCheckpointTimeSpan = currentCheckpoint.TimePoint - previousCheckpoint.TimePoint;
                CheckpointAggregate checkpointAggregateResultObject = GetCheckpointAggregate(currentCheckpoint.CheckpointName);

                checkpointAggregateResultObject.AggregateCheckpoint(momentCheckpointTimeSpan, currentCheckpoint);
                previousCheckpoint = currentCheckpoint;
            }
        }

        private CheckpointAggregate GetCheckpointAggregate(string checkpointName)
        {
            CheckpointAggregate result;
            if (!CheckpointAggregates.TryGetValue(checkpointName, out result))
            {
                result = new CheckpointAggregate(checkpointName);
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