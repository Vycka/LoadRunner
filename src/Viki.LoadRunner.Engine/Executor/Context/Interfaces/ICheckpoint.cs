using System;

namespace Viki.LoadRunner.Engine.Executor.Context.Interfaces
{
    /// <summary>
    /// Checkpoint acts as meassurement point in test iteration
    /// In successful iteration - ResultContext will contain 4 system checkpoints:
    /// 
    /// Checkpoint.IterationSetupCheckpointName
    ///  * Created before calling ScenarioSetup()
    /// Checkpoint.IterationStartCheckpointName
    ///  * Created before starting ExecuteScenario()
    ///  * Also at this time the timer will get started
    /// Checkpoint.IterationEndCheckpointName
    ///  * Created after successful ExecuteScenario() execution
    ///  * It will contain total ExecuteScenario() execution time
    ///  * Failed iterations won't have this checkpoint.
    ///  * Timer will also stop here
    /// Checkpoint.IterationTearDownCheckpointName
    ///  * Created before calling IterationTearDown()
    /// 
    /// If there is unhandled exception in Setup, Execute or Teardown steps, it will get logged to the last created checkpoint.
    ///  * E.g. if test fails in the middle of ExecuteScenario() and there are no custom checkpoints defined. Error will get logged to [Checkpoint.IterationStartCheckpointName] checkpoint.
    /// </summary>
    public interface ICheckpoint
    {
        /// <summary>
        /// Name of checkpoint
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Timepint of iteration when checkpoint was taken
        /// </summary>
        TimeSpan TimePoint { get; }

        /// <summary>
        /// If executed code below checkpoint creation throws error.
        /// Last previously created checkpoint will have this property set with thrown exception.
        /// </summary>
        Exception Error { get; }
    }
}