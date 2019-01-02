using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public interface IConsumer<T>
    {
        bool TryLockBatch(out IReadOnlyList<T> batch);

        void ReleaseBatch();

        bool Available { get; }
    }
}