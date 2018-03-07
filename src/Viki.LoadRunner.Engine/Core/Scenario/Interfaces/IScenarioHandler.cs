namespace Viki.LoadRunner.Engine.Core.Scenario.Interfaces
{
    public interface IScenarioHandler
    {
        /// <summary>
        /// Initial setup
        /// </summary>
        /// <remarks>Called 1st (Initialize)</remarks>
        void Init();

        /// <summary>
        /// Prepares context for next iteration
        /// </summary>
        ///<remarks>Called 2nd (Before each Execute)</remarks>
        void PrepareNext();

        /// <summary>
        /// Executes iteration
        /// </summary>
        ///<remarks>Called 3rd (After each Prepare)</remarks>
        void Execute();

        /// <summary>
        /// Final cleanup
        /// </summary>
        /// <remarks>Called last (Cleanup)</remarks>
        void Cleanup();
    }
}