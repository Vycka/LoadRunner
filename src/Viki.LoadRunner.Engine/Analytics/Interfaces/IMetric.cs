namespace Viki.LoadRunner.Engine.Analytics.Interfaces
{
    public interface IMetric<in T>
    {
        /// <summary>
        /// Create new blank IMetric instance based on current instance settings (e.g. settings passed in the constructor in histogram setup)
        /// </summary>
        /// <returns></returns>
        IMetric<T> CreateNew();

        /// <summary>
        /// Aggregate row of raw data
        /// </summary>
        /// <param name="data">row of raw data</param>
        void Add(T data);

        /// <summary>
        /// Names of columns produced by this metric (order must match [Values] order)
        /// </summary>
        string[] ColumnNames { get; }
        /// <summary>
        /// Values produced by this metric (order must match [ColumnNames] order)
        /// </summary>
        object[] Values { get; }
    }
}