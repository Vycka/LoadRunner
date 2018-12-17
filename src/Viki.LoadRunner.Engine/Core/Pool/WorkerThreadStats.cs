using Viki.LoadRunner.Engine.Core.Pool.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Pool
{
    public struct WorkerThreadStats : IThreadPoolStats
    {
        private readonly int _createdThreadCount;
        private readonly int _initializedTheadCount;
        private readonly int _idleThreadCount;

        public int CreatedThreadCount => _createdThreadCount;
        public int InitializedThreadCount => _initializedTheadCount;
        public int IdleThreadCount => _idleThreadCount;

        public WorkerThreadStats(IThreadPoolStats reference)
        {
            _createdThreadCount = reference.CreatedThreadCount;
            _initializedTheadCount = reference.InitializedThreadCount;
            _idleThreadCount = reference.IdleThreadCount;
        }
    }
}