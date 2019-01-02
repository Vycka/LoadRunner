using Viki.LoadRunner.Engine.Core.State.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Pool.Interfaces
{
    public interface IThreadPool 
    {
        void StartWorkersAsync(int count);
        void StopWorkersAsync(int count);
    }

    public static class ThreadPoolExtensions
    {
        public static void SetWorkerCountAsync(this IThreadPool context, ITestState state, int threadCount)
        {
            int delta = threadCount - state.ThreadPool.CreatedThreadCount;
            if (delta != 0)
            {
                if (delta > 0)
                    context.StartWorkersAsync(delta);
                else
                    context.StopWorkersAsync(-delta);
            }
        }
    }
}