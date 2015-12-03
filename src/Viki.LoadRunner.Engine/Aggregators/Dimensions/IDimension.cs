using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Dimensions
{
    public interface IDimension
    {
        /// <summary>
        /// Build dimension key by current provided TestContextResult
        /// </summary>
        /// <param name="result">current iteration result</param>
        /// <returns>String dimension key</returns>
        string GetKey(TestContextResult result);


        // TODO: Think of way to avoid having SetBegin in dimension

        /// <summary>
        /// Signals new load test execution start and its start utc time
        /// </summary>
        /// <param name="testBeginTime">Load test execution start utc time</param>
        void SetBegin(DateTime testBeginTime);
    }
}