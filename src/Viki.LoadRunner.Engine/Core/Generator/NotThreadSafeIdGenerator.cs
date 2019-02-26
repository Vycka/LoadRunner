using Viki.LoadRunner.Engine.Core.Generator.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Generator
{
    public class NotThreadSafeIdGenerator : IUniqueIdGenerator<int>
    {
        public int Next()
        {
            Current = Current + 1;

            return Current;
        }

        public int Current { get; private set; } = -1;
    }
}