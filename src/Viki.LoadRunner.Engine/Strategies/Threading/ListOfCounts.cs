using System;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class ListOfCounts :  IThreadingStrategy
    {
        private readonly TimeSpan _period;
        private readonly int[] _threadCountValues;

        private readonly int _lastValue;

        public ListOfCounts(TimeSpan period, params int[] threadCountValues)
        {
            if (threadCountValues == null)
                throw new ArgumentNullException(nameof(threadCountValues));
            if (threadCountValues.Length == 0)
                throw new ArgumentException("At least one value must be provided", nameof(threadCountValues));

            _period = period;
            _threadCountValues = threadCountValues;

            _lastValue = threadCountValues[threadCountValues.Length - 1];
        }

        public int InitialThreadCount => _threadCountValues[0];
        public int ThreadCreateBatchSize => 1;
        public void Setup(IThreadPoolContext context, IThreadPoolControl control)
        {
            control.SetWorkerCountAsync(InitialThreadCount);
        }

        public void HeartBeat(IThreadPoolContext context, IThreadPoolControl control)
        {
            long index = context.Timer.Value.Ticks / _period.Ticks;
            int result = 0;

            if (index < _threadCountValues.Length)
                result =  _threadCountValues[index];
            else
                result =  _lastValue;

            control.SetWorkerCountAsync(result);
        }
    }
}