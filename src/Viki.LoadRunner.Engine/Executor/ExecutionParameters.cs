using System;

namespace Viki.LoadRunner.Engine.Executor
{
    public class ExecutionParameters
    {
        public readonly TimeSpan MaxDuration;
        public readonly double MaxRequestsPerSecond;
        public readonly int FinishTimeoutMilliseconds;
        public readonly int MaxIterationsCount;
        public readonly int MinThreads;
        public readonly int MaxThreads;

        public ExecutionParameters(TimeSpan maxDuration, int minThreads = 10, int maxThreads = 10, double maxRequestsPerSecond = 10, int finishTimeoutMilliseconds = 180000, int maxIterationsCount = Int32.MaxValue)
        {
            MaxDuration = maxDuration;
            MaxRequestsPerSecond = maxRequestsPerSecond;
            FinishTimeoutMilliseconds = finishTimeoutMilliseconds;
            MaxIterationsCount = maxIterationsCount;
            MinThreads = minThreads;
            MaxThreads = maxThreads;
        }
    }
}