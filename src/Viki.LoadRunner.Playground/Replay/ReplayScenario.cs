using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scheduler.Interfaces;

namespace Viki.LoadRunner.Playground.Replay
{
    public class ReplayScenario : IReplayScenario<string>
    {
        // Defaults are for Ui Validate function
        private string _data = "DEFAULT";

        private TimeSpan _target = TimeSpan.Zero;

        public void ScenarioSetup(IIteration context)
        {
            Console.Out.WriteLine($"[{context.Timer.Value.TotalSeconds:F2}] Scenario Setup");
        }

        public void SetData(IData<string> data)
        {
            _data = data.Value;
            _target = data.TargetTime;

            TimeSpan fallenBehind = data.Timer.Value - data.TargetTime;

            if (fallenBehind > TimeSpan.FromSeconds(1))
                data.Skip = true;

            if (fallenBehind < TimeSpan.Zero)
                Console.WriteLine($@"[{data.Timer.Value.TotalSeconds:F2}] SetData: [{_data}] FALL BEHIND {fallenBehind}");

            Console.Out.WriteLine($"[{data.Timer.Value.TotalSeconds:F2}] SetData: [{_data}]");
        }


        public void IterationSetup(IIteration context)
        {
            context.UserData = _data;

            Console.Out.WriteLine($"[{context.Timer.Value.TotalSeconds:F2}][A:{_target.TotalSeconds:F2}] Iteration Setup");
        }

        public void ExecuteScenario(IIteration context)
        {
            Console.Out.WriteLine($"[{context.Timer.Value.TotalSeconds:F2}][A:{_target.TotalSeconds:F2}] Execute");
            Thread.Sleep(1000);
        }

        public void IterationTearDown(IIteration context)
        {
            Console.Out.WriteLine($"[{context.Timer.Value.TotalSeconds:F2}][A:{_target.TotalSeconds:F2}] Iteration End");
        }

        public void ScenarioTearDown(IIteration context)
        {
            Console.Out.WriteLine($"[{context.Timer.Value.TotalSeconds:F2}][A:{_target.TotalSeconds:F2}] Scenario End");
        }
    }
}