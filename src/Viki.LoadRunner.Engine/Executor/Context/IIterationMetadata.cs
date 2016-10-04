namespace Viki.LoadRunner.Engine.Executor.Context
{
    /// <summary>
    /// Only iteration metadata containing values (no meassurements)
    /// </summary>
    /// <typeparam name="TUserData">type of UserData it will carry</typeparam>
    public interface IIterationMetadata<TUserData>
    {
        /// <summary>
        /// Unique Iteration ID withing all worker-threads (Starts from zero)
        /// [Tip: If scenario fits - this reference ID can be used as index for test-data datasources]
        /// </summary>
        int GlobalIterationId { get; }

        /// <summary>
        /// Unique Iteration ID withing current instance of ILoadTestScenario (Starts from zero)
        /// [Tip: If scenario fits - this reference ID can be used as index for test-data datasources]
        /// </summary>
        int ThreadIterationId { get; }

        /// <summary>
        /// Unique worker-thread ID. It will stay the same throughout all ILoadTestScenario instance lifetime (Starts from zero)
        /// [Tip: If scenario fits - this reference ID can be used as index for test-data datasources]
        /// </summary>
        int ThreadId { get; }

        /// <summary>
        /// Field mainly used for passing data from test iteration to custom aggregation.
        /// </summary>
        TUserData UserData { get; set; }
    }
}