using Viki.LoadRunner.Engine.Strategies.Replay.Data;
using Viki.LoadRunner.Engine.Strategies.Replay.Data.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Extensions
{
    public static class ReplayDataReaderExtensions
    {
        public static DataItem Next(this IReplayDataReader reader, int threadId)
        {
            bool fakeRefStop = false;
            return reader.Next(threadId, ref fakeRefStop);
        }
    }
}
