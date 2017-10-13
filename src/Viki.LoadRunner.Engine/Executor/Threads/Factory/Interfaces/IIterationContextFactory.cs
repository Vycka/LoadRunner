using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Executor.Threads.Factory.Interfaces
{
    public interface IIterationContextFactory
    {
        IIterationContextControl Create();
    }
}