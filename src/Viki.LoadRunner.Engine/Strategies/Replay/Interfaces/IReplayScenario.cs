using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

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
        /// <remarks>It must not fail or it will stop the whole test execution.</remarks>
        /// <param name="data"></param>
        void SetData(TData data);
    }
}