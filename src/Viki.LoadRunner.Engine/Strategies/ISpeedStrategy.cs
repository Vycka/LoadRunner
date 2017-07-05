using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface ISpeedStrategy
    {
        void Next(IThreadContext context, ISchedule scheduler); // Must be thread safe

        void HeartBeat(IThreadPoolContext context);
    }
}