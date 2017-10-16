using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Strategy.Factory.Interfaces
{
    public interface IIterationContextFactory
    {
        IIterationContextControl Create();
    }
}