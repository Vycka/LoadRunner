using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces
{
    public interface IReplayScenarioFactory<in TData> : IFactory<IReplayScenario<TData>>
    {  
    }
}