namespace Viki.LoadRunner.Engine.Executor.Strategy.Scenario.Interfaces
{
    public interface IWork
    {
        void Init();
        void Execute();
        void Cleanup();

        void Stop();
    }
}