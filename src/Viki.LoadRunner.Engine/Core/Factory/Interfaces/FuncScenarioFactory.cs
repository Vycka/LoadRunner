using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Factory.Interfaces
{
    public delegate IScenario CreateScenarioDelegate(int threadId);

    public class FuncScenarioFactory : IScenarioFactory
    {
        private readonly CreateScenarioDelegate _factoryDelegate;

        public FuncScenarioFactory(CreateScenarioDelegate factoryDelegate)
        {
            _factoryDelegate = factoryDelegate;
        }

        public IScenario Create(int threadId)
        {
            return _factoryDelegate(threadId);
        }
    }
}