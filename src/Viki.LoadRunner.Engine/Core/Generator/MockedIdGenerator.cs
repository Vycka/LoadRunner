using Viki.LoadRunner.Engine.Core.Generator.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Generator
{
    public class MockedIdGenerator : IUniqueIdGenerator<int>
    {
        public MockedIdGenerator(int id)
        {
            Current = id;
        }

        public int Next()
        {
            return Current;
        }

        public int Current { get; }
    }
}