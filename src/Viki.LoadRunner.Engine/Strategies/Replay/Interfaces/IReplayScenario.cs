using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scheduler.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Interfaces
{
    /// <summary>
    /// IReplayScenario is extended version IScenario
    /// all logic is the same, except there is extra step before iteration to set the data given by IReplayDataReader
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IReplayScenario<in TData> : IScenario 
    {
        /// <summary>
        /// Set the data which which will be used for next iteration
        /// Call chain will be like this SetData() -> IterationSetup() -> ExecuteScenario() -> IterationTearDown().
        /// </summary>
        /// <remarks>
        /// * SetData() will get called as early as thread is free, and then it will wait for data.TargetTime to execute next three steps.
        /// * if data.TargetTime is bigger than data.Timer.Value. It means that execution is falling behing timeline.
        /// * It must not fail or it will stop the whole test execution.
        /// </remarks>
        /// <param name="data">Structure containing replay metadata related to next iteration</param>
        void SetData(IData<TData> data);
    }
}