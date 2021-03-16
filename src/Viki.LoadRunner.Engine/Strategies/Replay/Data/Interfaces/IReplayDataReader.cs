using Viki.LoadRunner.Engine.Core.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Data.Interfaces
{
    /// <summary>
    /// Data provider for ReplayStrategy.
    /// </summary>
    public interface IReplayDataReader
    {
        /// <summary>
        /// Gives the signal that execution is about to begin.
        /// </summary>
        void Begin(ITestState testState);

        /// <summary>
        /// Get next item to be executed in its corresponding order
        /// E.g. provided DataItem's must be already sorted by TimeStamp
        ///
        /// If implementation decides to block, then it must occasionally poll stop value to see if one needs to cancel execution (one can use SemiWait to easier handle this)
        /// after the stop, one can still return one more item to be executed (although its recommended to just return null to indicate end)
        /// </summary>
        /// <remarks>Must be thread safe</remarks>
        /// <returns></returns>
        DataItem Next(int threadId, ref bool stop);

        /// <summary>
        /// Gives the signal that test is ending, no more Next() calls after this.
        /// </summary>
        void End();
    }
}