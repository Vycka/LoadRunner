using System;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Threading;

namespace Viki.LoadRunner.Engine.Parameters
{
    public class LoadRunnerParameters
    {
        public ExecutionLimits Limits = new ExecutionLimits();
        public ISpeedStrategy SpeedStrategy = new FixedSpeed(Double.MaxValue);
        public IThreadingStrategy ThreadingStrategy = new SemiAutoThreadCount(10, 10);
    }
}