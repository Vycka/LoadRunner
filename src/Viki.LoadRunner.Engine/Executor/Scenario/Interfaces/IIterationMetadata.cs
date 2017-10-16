
namespace Viki.LoadRunner.Engine.Executor.Scenario.Interfaces
{
    /// <summary>
    /// Only iteration metadata containing values (no meassurements)
    /// </summary>
    /// <typeparam name="TUserData">type of UserData it will carry</typeparam>
    public interface IIterationMetadata<TUserData> : IIterationId
    {
        /// <summary>
        /// Field mainly used for passing data from test iteration to custom aggregation.
        /// </summary>
        TUserData UserData { get; set; }
    }
}