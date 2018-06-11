using Viki.LoadRunner.Engine.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Extensions
{
    public static class StrategyBuilderExtensions
    {
        /// <summary>
        /// Initialize IStrategy from this configuration and then LoadRunnerEngine it self using it. 
        /// </summary>
        /// <param name="builder">Strategy builder</param>
        public static LoadRunnerEngine Build(this IStrategyBuilder builder)
        {
            LoadRunnerEngine engine = new LoadRunnerEngine(builder.BuildStrategy());

            return engine;
        }

    }
}