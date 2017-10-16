namespace Viki.LoadRunner.Engine.Executor.Worker.Interfaces
{
    public interface IErrorHandler
    {
        void Assert();

        void Register(IWorkerThread thread);
    }
}