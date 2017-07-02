using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies
{
    public interface ISpeedStrategy
    {
        void Next(IThreadContext context, IIterationControl control); // Must be thread safe

        void Adjust(CoordinatorContext context);
    }
}