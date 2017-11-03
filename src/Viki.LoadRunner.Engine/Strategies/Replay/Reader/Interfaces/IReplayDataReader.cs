namespace Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces
{
    /// <summary>
    /// Data provider for ReplayStrategy.
    /// </summary>
    public interface IReplayDataReader
    {
        /// <summary>
        /// Gives the signal that execution is about to begin.
        /// </summary>
        void Begin();

        /// <summary>
        /// Get next item to be executed in its corresponding order
        /// E.g. provided DataItem's must be already sorted by TimeStamp
        /// </summary>
        /// <remarks>Must be thread safe</remarks>
        /// <returns></returns>
        DataItem Next();

        /// <summary>
        /// Gives the signal that test is ending, no more Next() calls after this.
        /// </summary>
        void End();
    }
}