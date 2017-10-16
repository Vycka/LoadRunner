namespace Viki.LoadRunner.Engine.Executor.Worker.Interfaces
{
    public interface IWork
    {
        void Init();
        void Execute();
        void Cleanup();

        void Stop();
    }
}