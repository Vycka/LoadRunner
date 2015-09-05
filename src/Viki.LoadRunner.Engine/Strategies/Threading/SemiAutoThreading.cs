using System;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class SemiAutoThreading : IThreadingStrategy
    {
        private readonly int _maxThreadCount;

        public SemiAutoThreading(int minThreadCount, int maxThreadCount)
        {
            InitialThreadCount = minThreadCount;
            _maxThreadCount = maxThreadCount;
        }

        public int InitialThreadCount { get; private set; }

        public int ThreadCreateBatchSize { get; } = 1;

        public int GetAllowedThreadCount(TimeSpan testExecutionTime)
        {
            return _maxThreadCount;
        }
    }
}