using System.Threading;

namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public class IdFactory : IUniqueIdFactory<int>
    {
        private int _id = -1;

        public int Next()
        {
            int result = Interlocked.Increment(ref _id);

            return result;
        }

        public int Current => _id;
    }
}