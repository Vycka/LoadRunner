using System;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Scheduler.Interfaces
{
    /// <summary>
    /// Contains information used for seting up upcomming replay scenario iteration.
    /// </summary>
    /// <typeparam name="TData">scenario data value type</typeparam>
    public interface IData<out TData>
    {
        /// <summary>
        /// Global test timer
        /// </summary>
        ITimer Timer { get; }

        /// <summary>
        /// Adjusted target time based on provided speed multiplier
        /// if (TargetTime &lt; Timer.Value) it means that scenario is falling behind the timeline
        /// </summary>
        TimeSpan TargetTime { get; }

        /// <summary>
        /// Test data asociated with this iteration
        /// </summary>
        TData Value { get; }

        /// <summary>
        /// Setting to true will skip this iteration Setup/Execute/Teardown steps
        /// Iteration result will only contain (Checkpoint.Names.Skip [default value "ITERATION_SKIP"]) checkpoint
        /// </summary>
        /// <remarks>It can be used to handle execution timeline falling behind and skip few requests</remarks>
        bool Skip { get; set; }  
    }
}