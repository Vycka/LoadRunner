using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface ISpeedStrategy
    {
        void Next(IThreadContext context, IScheduler scheduler); // Must be thread safe

        void Adjust(IThreadPoolContext context);
    }
}