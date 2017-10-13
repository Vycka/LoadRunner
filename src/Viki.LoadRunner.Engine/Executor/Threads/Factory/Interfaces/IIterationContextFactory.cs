using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Context.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Threads.Factory.Interfaces
{
    public interface IIterationContextFactory
    {
        IIterationContextControl Create();
    }
}