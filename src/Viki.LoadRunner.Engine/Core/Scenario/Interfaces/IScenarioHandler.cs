namespace Viki.LoadRunner.Engine.Core.Scenario.Interfaces
{
    public interface IScenarioHandler
    {
        /// <summary>
        /// Initial setup
        /// </summary>
        /// <remarks>Called 1st (Initialize) [Only once]</remarks>
        void Init();

        /// <summary>
        /// Setup context for next iteration.
        /// (Like assigning next Global/Thread IterationId's, as this information will be used in applying ISpeedStrategy)
        /// </summary>
        ///<remarks>Called first before each iteration.</remarks>
        void PrepareNext();

        /// <summary>
        /// Once execution is scheduled, this gets called early to allow IterationSetup prepare early.
        /// </summary>
        /// <remarks>Called after PrepareNext and as early as possible from scheduled execution time</remarks>
        void Warmup();

        /// <summary>
        /// Executes iteration
        /// </summary>
        ///<remarks>Called 3rd [After each Warmup]</remarks>
        void Execute();

        /// <summary>
        /// Final cleanup
        /// </summary>
        /// <remarks>Called last (Cleanup) [Only once]</remarks>
        void Cleanup();
    }
}