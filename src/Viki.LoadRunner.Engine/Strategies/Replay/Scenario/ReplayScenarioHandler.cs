using System;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Worker;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Scheduler;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Scenario
{
    public class ReplayScenarioHandler<TData> : ScenarioHandler, IReplayScenarioHandler
    {
        private readonly IReplayScenario<TData> _scenario;

        private readonly DataContext<TData> _dataContext;

        public ReplayScenarioHandler(IUniqueIdFactory<int> globalIdFactory, IReplayScenario<TData> scenario, IIterationControl context) 
            : base(globalIdFactory, scenario, context)
        {
            _scenario = scenario;

            _dataContext = new DataContext<TData>
            {
                Timer = context.Timer,
                Skip = false
            };
        }

        public void SetData(object data, TimeSpan target)
        {
            _dataContext.Set((TData)data, target);
            
            _scenario.SetData(_dataContext);
        }

        public new void Execute()
        {
            if (_dataContext.Skip)
            {
                _context.Checkpoint(Checkpoint.Names.Skip);
            }
            else
            {
                base.Execute();
            }
        }
    }
}