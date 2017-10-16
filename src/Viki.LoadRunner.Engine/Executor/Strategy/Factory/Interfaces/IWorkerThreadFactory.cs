using Viki.LoadRunner.Engine.Executor.Strategy.Workers.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.Factory.Interfaces
{
    public interface IWorkerThreadFactory
    {
        IWorkerThread Create();
    }
}