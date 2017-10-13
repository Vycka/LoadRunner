namespace Viki.LoadRunner.Engine.Executor.Threads.Scenario.Interfaces
{
    public interface IWork
    {
        void Init();
        void Wait();
        void Execute();
        void Cleanup();

        void Stop();
    }
}