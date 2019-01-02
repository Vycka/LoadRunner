using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit
{
    public class ErrorLimit : ILimitStrategy
    {
        private readonly int _errorsThreshold;

        public ErrorLimit(int errorsThreshold)
        {
            _errorsThreshold = errorsThreshold;
        }

        public bool StopTest(ITestState state)
        {
            return state.Counters.ErrorCount >= _errorsThreshold;
        }
    }
}