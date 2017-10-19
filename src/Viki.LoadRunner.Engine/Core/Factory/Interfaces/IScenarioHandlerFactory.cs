using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory.Interfaces
{
    public interface IScenarioHandlerFactory
    {
        IScenarioHandler Create(IIterationControl iterationContext);
    }
}