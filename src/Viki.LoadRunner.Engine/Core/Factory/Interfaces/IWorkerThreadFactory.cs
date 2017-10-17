using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory.Interfaces
{
    public interface IWorkerThreadFactory
    {
        IWorkerThread Create();
    }
}