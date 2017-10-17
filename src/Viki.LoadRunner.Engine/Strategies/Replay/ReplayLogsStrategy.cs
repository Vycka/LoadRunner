using Viki.LoadRunner.Engine.Strategies.Custom;
using Viki.LoadRunner.Engine.Strategies.Custom.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Replay
{
    public class ReplayLogsStrategy : CustomStrategy

    {
        public ReplayLogsStrategy(ICustomStrategySettings settings) : base(settings)
        {
        }
    }
}