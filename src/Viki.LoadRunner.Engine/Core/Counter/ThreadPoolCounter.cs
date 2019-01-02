using Viki.LoadRunner.Engine.Core.Counter.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Counter
{
    public class ThreadPoolCounter : IThreadPoolCounter
    {
        public int CreatedThreadCount => _created.Value;
        public int InitializedThreadCount => _initialized.Value;
        public int IdleThreadCount => _idle.Value;

        private readonly ICounter _created, _initialized, _idle;

        public ThreadPoolCounter()
        {
            _created = new ThreadSafeCounter();
            _initialized = new ThreadSafeCounter();
            _idle = new ThreadSafeCounter();
        }

        public void AddIdle(int count)
        {
            _idle.Add(count);
        }

        public void AddInitialized(int count)
        {
            _initialized.Add(count);
        }

        public void AddCreated(int count)
        {
            
            _created.Add(count);
        }
    }
}