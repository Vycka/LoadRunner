using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregates.Utils
{
    public class AggregatedCheckpoint
    {
        public readonly List<Exception> Errors;

        public TimeSpan SummedMomentTime;
        public TimeSpan SummedTotalTime;

        public DateTime FirsIterationBeginTime;
        public DateTime LastIterationBeginTime;
        public DateTime FirstIterationEndTime;
        public DateTime LastIterationEndTime;

        public readonly string Name;

        public TimeSpan MomentMin { get; private set; }

        public TimeSpan MomentMax { get; private set; }


        public TimeSpan TotalMin { get; private set; }

        public TimeSpan TotalMax { get; private set; }

        public int Count { get; private set; }

        public AggregatedCheckpoint(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;

            Errors = new List<Exception>();
            SummedMomentTime = new TimeSpan();

            MomentMin = TimeSpan.MaxValue;
            MomentMax = TimeSpan.MinValue;
            TotalMin = TimeSpan.MaxValue;
            TotalMax = TimeSpan.MinValue;

            FirsIterationBeginTime = DateTime.MaxValue;
            FirstIterationEndTime = DateTime.MaxValue;
            LastIterationBeginTime = DateTime.MinValue;
            LastIterationEndTime = DateTime.MinValue;

            Count = 0;
        }

        public void AggregateCheckpoint(TimeSpan momentDuration, Checkpoint checkpoint, TestContextResult resultContext)
        {
            Count++;

            if (checkpoint.Error != null)
                Errors.Add(checkpoint.Error);

            SummedMomentTime += momentDuration;
            SummedTotalTime += checkpoint.TimePoint;

            if (MomentMin > momentDuration)
                MomentMin = momentDuration;

            if (MomentMax < momentDuration)
                MomentMax = momentDuration;

            if (TotalMin > checkpoint.TimePoint)
                TotalMin = checkpoint.TimePoint;

            if (TotalMax < checkpoint.TimePoint)
                TotalMax = checkpoint.TimePoint;

            if (FirsIterationBeginTime > resultContext.IterationStarted)
                FirsIterationBeginTime = resultContext.IterationStarted;

            if (LastIterationEndTime < resultContext.IterationFinished)
                LastIterationEndTime = resultContext.IterationFinished;
        }
    }
}