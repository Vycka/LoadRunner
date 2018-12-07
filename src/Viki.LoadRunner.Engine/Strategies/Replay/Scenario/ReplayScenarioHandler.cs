using System;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scheduler;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Scenario
{
    public class ReplayScenarioHandler<TData> : ScenarioHandler, IReplayScenarioHandler
    {
        private readonly IReplayScenario<TData> _scenario;

        private readonly DataContext<TData> _dataContext;

        public ReplayScenarioHandler(IGlobalCountersControl globalCounters, IReplayScenario<TData> scenario, IIterationControl context) 
            : base(globalCounters, scenario, context)
        {
            _scenario = scenario;

            _dataContext = new DataContext<TData>
            {
                Timer = context.Timer,
                Execute = true
            };
        }

        public bool SetData(object data, TimeSpan target)
        {
            _dataContext.Set((TData)data, target);
            
            _scenario.SetData(_dataContext);

            return _dataContext.Execute;
        }

        public new void Execute()
        {
            if (_dataContext.Execute)
            {
                base.Execute();
            }
            else
            {
                _context.Checkpoint(Checkpoint.Names.Skip);
            }
        }
    }
}