using System;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface ISpeedStrategy
    {
        void Next(IThreadContextWat context, ISchedule scheduler); // Must be thread safe

        void HeartBeat(IThreadPoolContext context);
    }
}