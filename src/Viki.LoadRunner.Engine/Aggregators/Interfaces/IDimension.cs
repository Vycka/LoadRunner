using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Interfaces
{
    public interface IDimension
    {
        /// <summary>
        /// DisplayName/Key of the column
        /// </summary>
        string DimensionName { get; }

        /// <summary>
        /// Build dimension key by current provided TestContextResult
        /// </summary>
        /// <param name="result">current iteration result</param>
        /// <returns>String dimension key</returns>
        string GetKey(IResult result);
    }
}