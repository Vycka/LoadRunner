namespace Viki.LoadRunner.Engine.Strategies.Interfaces
{
    public interface IUserDataFeature : IStrategyBuilder
    {
        /// <summary>
        /// Initial user data which will be passed to created thread contexts. (context.UserData)
        /// </summary>
        object InitialUserData { get; set; }
    }
}