using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Threads.Workers.Interfaces
{
    public interface IErrorHandler
    {
        void Assert();

        void Register(IWorkerThread thread);
    }
}