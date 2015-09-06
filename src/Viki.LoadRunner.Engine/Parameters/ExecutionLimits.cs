using System;

namespace Viki.LoadRunner.Engine.Parameters
{
    public class ExecutionLimits
    {
        public TimeSpan MaxDuration = TimeSpan.FromSeconds(30);
        public TimeSpan FinishTimeout = TimeSpan.FromSeconds(60);
        public int MaxIterationsCount = Int32.MaxValue;
    }
}