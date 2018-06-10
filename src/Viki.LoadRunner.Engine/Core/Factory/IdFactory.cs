using System.Threading;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory
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
        public int Value => _id;


    }
}