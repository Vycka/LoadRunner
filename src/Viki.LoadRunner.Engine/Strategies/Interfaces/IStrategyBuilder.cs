namespace Viki.LoadRunner.Engine.Strategies.Interfaces
{
    public interface IStrategyBuilder
    {
        IStrategy BuildStrategy();

        /// <summary>
        /// Duplicates configuration builder having own configuration lists. But registered configuration instances will still be the same.
        /// </summary>
        /// <returns>New instance of IStrategyBuilder</returns>
        IStrategyBuilder ShallowCopy();
    }
}