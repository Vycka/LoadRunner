using System;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Threading;

namespace Viki.LoadRunner.Engine.Parameters
{
    public class LoadRunnerParameters
    {

        public readonly TimeSpan MaxDuration;
        public readonly double MaxRequestsPerSecond;
        public readonly int FinishTimeoutMilliseconds;
        public readonly int MaxIterationsCount;

        public IThreadingStrategy ThreadingStrategy = new SemiAutoThreading(10, 10);

        public LoadRunnerParameters(
            TimeSpan maxDuration,
            double maxRequestsPerSecond = Double.MaxValue,
            int finishTimeoutMilliseconds = 180000,
            int maxIterationsCount = Int32.MaxValue
        )
        {
            MaxDuration = maxDuration;
            MaxRequestsPerSecond = maxRequestsPerSecond;
            FinishTimeoutMilliseconds = finishTimeoutMilliseconds;
            MaxIterationsCount = maxIterationsCount;
        }
    }
}