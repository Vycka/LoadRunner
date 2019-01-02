using System.Threading;
using Viki.LoadRunner.Engine.Core.Counter.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Counter
{
    public class ThreadSafeCounter : ICounter
    {
        public int Value => _value;

        private int _value = 0;

        public int Add(int count)
        {
            return Interlocked.Add(ref _value, count);
        }
    }
}