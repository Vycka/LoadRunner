using System;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scheduler.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Scheduler
{
    public class DataContext<TData> : IData<TData>
    {
        /// <summary>
        /// Set information about upcomming iteration.
        /// Both passed values are only use for passing it to scenario it self
        /// </summary>
        /// <param name="value">data value</param>
        /// <param name="target">target time</param>
        public void Set(TData value, TimeSpan target)
        {
            Value = value;
            TargetTime = target;
            Skip = false;
        }

        /// <summary>
        /// Global test timer
        /// </summary>
        public ITimer Timer { get; set; }

        /// <summary>
        /// Adjusted target time based on provided speed multiplier
        /// if (TargetTime &lt; Timer.Value) it means that scenario is falling behind the timeline
        /// </summary>
        public TimeSpan TargetTime { get; set; }

        /// <summary>
        /// Test data asociated with this iteration
        /// </summary>
        public TData Value { get; set; }

        /// <summary>
        /// Setting to true will skip this iteration Setup/Execute/Teardown steps
        /// Iteration result will only contain (Checkpoint.Names.Skip [default value "ITERATION_SKIP"]) checkpoint
        /// </summary>
        /// <remarks>It can be used to handle execution timeline falling behind and skip few requests</remarks>
        public bool Skip { get; set; }
    }
}