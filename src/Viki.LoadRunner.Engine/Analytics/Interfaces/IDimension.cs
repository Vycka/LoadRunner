namespace Viki.LoadRunner.Engine.Analytics.Interfaces
{
    public interface IDimension<in T>
    {
        /// <summary>
        /// DisplayName/Key of the column
        /// </summary>
        string DimensionName { get; }

        /// <summary>
        /// Build dimension key by provided row of raw data
        /// </summary>
        /// <param name="data">row of raw data</param>
        /// <returns>String dimension key</returns>
        string GetKey(T data);
    }
}
