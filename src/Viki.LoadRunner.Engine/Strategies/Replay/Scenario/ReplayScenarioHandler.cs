using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Scenario
{
    public class ReplayScenarioHandler<TData> : ScenarioHandler, IReplayScenarioHandler
    {
        public IReplayScenario<TData> Scenario { get; }

        public ReplayScenarioHandler(IUniqueIdFactory<int> globalIdFactory, IReplayScenario<TData> scenario, IIterationControl context) 
            : base(globalIdFactory, scenario, context)
        {
            Scenario = scenario;
        }

        public void SetData(object data)
        {
            Scenario.SetData((TData)data);
        }
    }
}