using Viki.LoadRunner.Engine.Core.State.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces
{
    /// <summary>
    /// Limit-Strategy tells when test execution should be stopped.
    /// </summary>
    public interface ILimitStrategy
    {
        /// <summary>
        /// StopTest() gets called at every HeartBeat from executor (by default every 100ms.)
        /// once returned value is [true] engine will initiate graceful shutdown respecting [FinishTimeout] value.
        /// </summary>
        bool StopTest(ITestState state);
    }
}