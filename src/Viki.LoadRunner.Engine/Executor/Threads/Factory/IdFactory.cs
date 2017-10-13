using System.Threading;
using Viki.LoadRunner.Engine.Executor.Threads.Factory.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Threads.Factory
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