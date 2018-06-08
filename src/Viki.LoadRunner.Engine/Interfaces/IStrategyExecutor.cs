using System;

namespace Viki.LoadRunner.Engine.Interfaces
{
    public interface IStrategyExecutor 
    {
        /// <summary>
        /// Start LoadTest execution on main thread. This blocks until test execution is finished by defined rules if any.
        /// </summary>
        void Run();
    }
}