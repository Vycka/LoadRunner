using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Core.Collector.Pipeline
{
    public interface IConsumer<T>
    {
        IReadOnlyList<T> TryLockBatch();

        void ReleaseBatch();

        bool Completed { get; }
    }
}