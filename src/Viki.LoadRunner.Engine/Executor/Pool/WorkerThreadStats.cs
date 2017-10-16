using Viki.LoadRunner.Engine.Executor.Pool.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Pool
{
    public struct WorkerThreadStats : IThreadPoolStats
    {
        private readonly short _createdThreadCount;
        private readonly short _initializedTheadCount;
        private readonly short _idleThreadCount;

        public int CreatedThreadCount => _createdThreadCount;
        public int InitializedThreadCount => _initializedTheadCount;
        public int IdleThreadCount => _idleThreadCount;

        public WorkerThreadStats(IThreadPoolStats reference)
        {
            _createdThreadCount = (short)reference.CreatedThreadCount;
            _initializedTheadCount = (short)reference.InitializedThreadCount;
            _idleThreadCount = (short) reference.IdleThreadCount;
        }
    }
}