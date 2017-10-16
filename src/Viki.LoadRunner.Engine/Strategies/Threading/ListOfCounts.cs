using System;
using Viki.LoadRunner.Engine.Executor.Strategy.Pool.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;

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

        public void Setup(IThreadPool pool)
        {
            pool.StartWorkersAsync(InitialThreadCount);
        }

        public void HeartBeat(IThreadPool pool, ITestState state)
        {
            long index = state.Timer.Value.Ticks / _period.Ticks;
            int result = 0;

            if (index < _threadCountValues.Length)
                result =  _threadCountValues[index];
            else
                result =  _lastValue;

            pool.SetWorkerCountAsync(state, result);
        }
    }
}