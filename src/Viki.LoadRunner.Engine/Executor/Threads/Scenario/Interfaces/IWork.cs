namespace Viki.LoadRunner.Engine.Executor.Threads.Scenario.Interfaces
{
    public interface IWork
    {
        void Init();
        void Execute();
        void Cleanup();

        void Stop();
    }
}