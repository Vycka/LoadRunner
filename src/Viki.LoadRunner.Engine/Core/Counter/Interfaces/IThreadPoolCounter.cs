using Viki.LoadRunner.Engine.Core.Pool.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Counter.Interfaces
{
    public interface IThreadPoolCounter : IThreadPoolStats
    {
        void AddIdle(int count);

        void AddInitialized(int count);

        void AddCreated(int count);
    }
}