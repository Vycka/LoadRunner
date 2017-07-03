using System;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed.PriorityStrategy
{
    public abstract class PriorityStrategy : IIterationControl
    {
        // wtf i am oin?
        public abstract void Idle(TimeSpan delay);


        public abstract void Execute(TimeSpan at);


        public abstract void Execute();

    }
}