namespace Viki.LoadRunner.Engine.Executor.Threads
{

    // TODO: Handle no threads, try delete  threads from lowest index
    // due to how ISpeedStrategy by working thread count will look like
    public interface IThreadPoolControl
    {
        void StartWorkersAsync(int threadCount);
        void StopWorkersAsync(int threadCount);

        int CreatedThreadCount { get; }
    }

    public static class ThreadPoolControlExtensions
    {
        public static void SetWorkerCountAsync(this IThreadPoolControl control, int threadCount)
        {
            int delta = threadCount - control.CreatedThreadCount;
            if (delta != 0)
            {
                if (delta > 0)
                    control.StartWorkersAsync(delta);
                else
                    control.StopWorkersAsync(-delta);
            }
        }
    }
}