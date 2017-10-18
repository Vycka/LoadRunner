namespace Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces
{
    public interface IReplayDataReader
    {
        void Begin();

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Must be thread safe</remarks>
        /// <returns></returns>
        DataItem Next();

        void End();
    }
}