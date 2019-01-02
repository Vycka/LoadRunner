namespace Viki.LoadRunner.Engine.Core.Scenario.Interfaces
{
    /// <summary>
    /// IScenario defines exection for a single thread.
    /// After created, it will call ScenarioSetup() shortly
    /// In successful iteration the calls will go like this:  IterationSetup() -> ExecuteScenario() -> IterationTearDown()
    /// When thread is stopping after IterationTearDown(), ScenarioTearDown() will get called for graceful cleanup.
    /// </summary>
    public interface IScenario
    {
        /// <summary>
        /// ScenarioSetup is used to Initialize instance.
        /// It is called first after the IScenario instance is created.
        /// It must succeed, unhandled errors here will stop the thread abnormally and that will stop test execution.
        /// </summary>
        /// <param name="context">Fixed context associated with current scenario instance</param>
        void ScenarioSetup(IIteration context);

        /// <summary>
        /// Iteration setup is called each time before new iteration.
        /// Purpose of this is to setup iteration-specific data before the meassurement timer is started.
        /// </summary>
        /// <param name="context">Fixed context associated with current scenario instance</param>
        void IterationSetup(IIteration context);

        /// <summary>
        /// ExecuteScenario is called after each successful IterationSetup() call
        /// Execution time is meassured for this ExecuteScenario()
        /// If IterationSetup() fails, ExecuteScenario() will be skipped
        /// </summary>
        /// <param name="context">Fixed context associated with current scenario instance</param>
        void ExecuteScenario(IIteration context);

        /// <summary>
        /// IterationTearDown is called after each IterationSetup() -> ExecuteScenario() try.
        /// It doesn't matter if they fail or succeed in any way.
        /// IterationTearDown() will still get called to allow cleaning up the iteration.
        /// </summary>
        /// <param name="context">Fixed context associated with current scenario instance</param>
        void IterationTearDown(IIteration context);

        /// <summary>
        /// ScenarioTearDown is called once this thread is marked for stop.
        /// It must succeed, unhandled errors here will stop the thread abnormally and that will stop test execution.
        /// </summary>
        /// <param name="context">Fixed context associated with current scenario instance</param>
        void ScenarioTearDown(IIteration context);
    }
}