using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;

namespace Viki.LoadRunner.Playground.Replay
{
    public class ReplayScenario : IReplayScenario<string>
    {
        public void SetData(string data)
        {
            throw new System.NotImplementedException();
        }

        public void ScenarioSetup(IIteration context)
        {
            throw new System.NotImplementedException();
        }

        public void IterationSetup(IIteration context)
        {
            throw new System.NotImplementedException();
        }

        public void ExecuteScenario(IIteration context)
        {
            throw new System.NotImplementedException();
        }

        public void IterationTearDown(IIteration context)
        {
            throw new System.NotImplementedException();
        }

        public void ScenarioTearDown(IIteration context)
        {
            throw new System.NotImplementedException();
        }
    }
}