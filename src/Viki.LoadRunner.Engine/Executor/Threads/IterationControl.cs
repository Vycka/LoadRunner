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

        public IterationAction Action { get; private set; }
        public TimeSpan TimeValue { get; private set; }

        public void Idle(TimeSpan delay)
        {
            Action = IterationAction.Idle;
            TimeValue = delay;
        }

        public void Execute()
        {
            Action = IterationAction.Execute;
            TimeValue = TimeSpan.Zero;
        }

        public void Execute(TimeSpan delay)
        {
            Action = IterationAction.Execute;
            TimeValue = delay;
        }
    }
}