using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Interfaces
{
    public interface IMetric
    {
        /// <summary>
        /// Create new blank IMetric instance based on current instance settings (e.g. settings passed in the constructor in histogram setup)
        /// </summary>
        /// <returns></returns>
        IMetric CreateNew();

        /// <summary>
        /// New iteration result received event
        /// </summary>
        /// <param name="result">Iteration result</param>
        void Add(IResult result);

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