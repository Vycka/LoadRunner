namespace Viki.LoadRunner.Engine.Executor.Strategy.Workers.Interfaces
{
    public interface IErrorHandler
    {
        void Assert();

        void Register(IWorkerThread thread);
    }
}