using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Executor.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Factory.Interfaces
{
    public interface IScenarioHandlerFactory
    {
        IScenarioHandler Create(IIterationControl iterationContext);
    }
}