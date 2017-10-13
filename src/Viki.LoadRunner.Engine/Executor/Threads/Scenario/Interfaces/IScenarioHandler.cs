namespace Viki.LoadRunner.Engine.Executor.Threads.Scenario.Interfaces
{
    public interface IScenarioHandler
    {
        /// <summary>
        /// Initial setup
        /// </summary>
        void Init();

        /// <summary>
        /// Final cleanup
        /// </summary>
        void Cleanup();

        /// <summary>
        /// Prepares context for next iteration
        /// </summary>
        void PrepareNext();

        /// <summary>
        /// Executes iteration
        /// </summary>
        void Execute();
    }
}