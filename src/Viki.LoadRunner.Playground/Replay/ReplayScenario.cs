using System;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Timer;
using Viki.LoadRunner.Engine.Core.Timer.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;

namespace Viki.LoadRunner.Playground.Replay
{
    public class ReplayScenario : IReplayScenario<string>
    {
        // Defaults are for Ui Validate function
        private ITimer _timer = new ExecutionTimer();
        private string _data = "DEFAULT";

        public async void ScenarioSetup(IIteration context)
        {
            int x = await Task.FromResult<int>(0);
            _timer = context.Timer;

            Console.Out.WriteLine($"[{_timer.Value.TotalSeconds:F2}] Scenario Setup");
        }

        public void SetData(string data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _data = data;

            Console.Out.WriteLine($"[{_timer.Value.TotalSeconds:F2}] SetData: [{data}]");
        }


        public void IterationSetup(IIteration context)
        {
            context.UserData = _data;

            Console.Out.WriteLine($"[{_timer.Value.TotalSeconds:F2}] Iteration Setup");
        }

        public void ExecuteScenario(IIteration context)
        {
            Console.Out.WriteLine($"[{_timer.Value.TotalSeconds:F2}] Execute");
        }

        public void IterationTearDown(IIteration context)
        {
            Console.Out.WriteLine($"[{_timer.Value.TotalSeconds:F2}] Iteration End");
        }

        public void ScenarioTearDown(IIteration context)
        {
            Console.Out.WriteLine($"[{_timer.Value.TotalSeconds:F2}] Scenario End");
        }
    }
}