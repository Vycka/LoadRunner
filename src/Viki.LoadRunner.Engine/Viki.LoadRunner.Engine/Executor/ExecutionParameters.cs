using System;

namespace Viki.LoadRunner.Engine.Executor
{
    public class ExecutionParameters
    {
        public readonly TimeSpan MaxDuration;
        public readonly int MaxRequestsPerSecond;
        public readonly int FinishTimeoutMilliseconds;
        public readonly int MinThreads;
        public readonly int MaxThreads;

        public ExecutionParameters(TimeSpan maxDuration, int minThreads = 10, int maxThreads = 100, int maxRequestsPerSecond = 10, int finishTimeoutMilliseconds = 180000)
        {
            MaxDuration = maxDuration;
            MaxRequestsPerSecond = maxRequestsPerSecond;
            FinishTimeoutMilliseconds = finishTimeoutMilliseconds;
            MinThreads = minThreads;
            MaxThreads = maxThreads;
        }
    }
}