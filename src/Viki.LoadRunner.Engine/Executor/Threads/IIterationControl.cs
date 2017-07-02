using System;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public interface IIterationControl
    {
        void Idle(TimeSpan delay);
        void Execute(TimeSpan at);
        void Execute();
    }
}