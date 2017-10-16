using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Factory.Interfaces
{
    public interface IIterationContextFactory
    {
        IIterationControl Create();
    }
}