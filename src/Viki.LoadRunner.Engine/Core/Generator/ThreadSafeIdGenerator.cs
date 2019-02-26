using System.Threading;
using Viki.LoadRunner.Engine.Core.Generator.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Generator
{
    public class ThreadSafeIdGenerator : IUniqueIdGenerator<int>
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