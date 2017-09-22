using System;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Framework;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface ISpeedStrategy
    {
        void Next(IIterationState state, ISchedule scheduler); // Must be thread safe

        void HeartBeat(ITestState state);
    }
}