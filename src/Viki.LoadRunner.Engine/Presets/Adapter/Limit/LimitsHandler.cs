using System;
using Viki.LoadRunner.Engine.Executor.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Presets.Adapter.Limit
{
    public class LimitsHandler : ILimitStrategy
    {
        private readonly ILimitStrategy[] _limits;

        public LimitsHandler(ILimitStrategy[] limits)
        {
            if (limits == null)
                throw new ArgumentNullException(nameof(limits));

            _limits = limits;
        }

        public bool StopTest(ITestState state)
        {
            for (int i = 0; i < _limits.Length; i++)
            {
                if (_limits[i].StopTest(state))
                    return true;
            }

            return false;
        }
    }
}