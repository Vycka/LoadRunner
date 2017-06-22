using System;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class ListOfCounts : IThreadingStrategy
    {
        private readonly TimeSpan _period;
        private readonly int[] _threadCountValues;

        private readonly int _lastValue;
        private readonly int _maxValue;

        public ListOfCounts(TimeSpan period, params int[] threadCountValues)
        {
            if (threadCountValues == null)
                throw new ArgumentNullException(nameof(threadCountValues));
            if (threadCountValues.Length == 0)
                throw new ArgumentException("At least one value must be provided", nameof(threadCountValues));

            _period = period;
            _threadCountValues = threadCountValues;

            _lastValue = threadCountValues[threadCountValues.Length - 1];
            _maxValue = threadCountValues.Max();
        }

        public int GetAllowedMaxWorkingThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            long index = testExecutionTime.Ticks / _period.Ticks;

            if (index < _threadCountValues.Length)
                return _threadCountValues[index];

            return _lastValue;
        }

        public int GetAllowedCreatedThreadCount(TimeSpan testExecutionTime, WorkerThreadStats workerThreadStats)
        {
            return _maxValue;
        }

        public int InitialThreadCount => _threadCountValues[0];
        public int ThreadCreateBatchSize => 1;
    }
}