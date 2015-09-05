using System;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class IncrementalThreading : IThreadingStrategy
    {
        private readonly TimeSpan _increasePeriod;

        public IncrementalThreading(TimeSpan increasePeriod, int increaseBatchSize, int initialThreadcount)
        {
            InitialThreadCount = initialThreadcount;
            _increasePeriod = increasePeriod;
            ThreadCreateBatchSize = increaseBatchSize;
        }

        public int InitialThreadCount { get; }
        public int ThreadCreateBatchSize { get; }
        public int GetAllowedThreadCount(TimeSpan testExecutionTime)
        {
            return(((int) (testExecutionTime.TotalMilliseconds/_increasePeriod.TotalMilliseconds)) * ThreadCreateBatchSize) + InitialThreadCount;
        }
    }
}