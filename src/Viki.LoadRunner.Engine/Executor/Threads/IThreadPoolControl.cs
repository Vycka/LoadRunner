namespace Viki.LoadRunner.Engine.Executor.Threads
{
    public interface IThreadPoolControl
    {
        void StartWorkersAsync(int threadCount);
        void StopWorkersAsync(int threadCount);

        int CreatedThreadCount { get; }
    }

    public static class IThreadPoolControlExtensions
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