using System;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class IterationControl : IIterationControl
    {
        public enum IterationAction
        {
            Idle,
            Execute
        }

        public IterationAction Action { get; protected set; }
        public TimeSpan TimeValue { get; protected set; }

        public virtual void Idle(TimeSpan delay)
        {
            Action = IterationAction.Idle;
            TimeValue = delay;
        }

        public virtual void Execute()
        {
            Action = IterationAction.Execute;
            TimeValue = TimeSpan.Zero;
        }

        public virtual void Execute(TimeSpan delay)
        {
            Action = IterationAction.Execute;
            TimeValue = delay;
        }
    }
}