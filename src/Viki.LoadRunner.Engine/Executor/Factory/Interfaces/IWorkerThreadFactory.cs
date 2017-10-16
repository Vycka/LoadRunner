using Viki.LoadRunner.Engine.Executor.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Factory.Interfaces
{
    public interface IWorkerThreadFactory
    {
        IWorkerThread Create();
    }
}