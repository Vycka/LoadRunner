using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory.Interfaces
{
    public interface IScenarioHandlerFactory
    {
        IScenarioHandler Create(IIterationControl iterationContext);
    }
}