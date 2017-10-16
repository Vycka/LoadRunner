﻿using Viki.LoadRunner.Engine.Executor.Strategy.Pool.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Threading
{
    public class FixedThreadCount : IThreadingStrategy
    {
        protected int ThreadCount;

        public FixedThreadCount(int threadCount)
        {
            ThreadCount = threadCount;
        }

        public void Setup(IThreadPool pool)
        {
            pool.StartWorkersAsync(ThreadCount);
        }

        public void HeartBeat(IThreadPool pool, ITestState state)
        {
            pool.SetWorkerCountAsync(state, ThreadCount);
        }
    }
}