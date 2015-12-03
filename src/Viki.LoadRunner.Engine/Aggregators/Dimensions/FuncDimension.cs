using System;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Dimensions
{
    /// <summary>
    /// Split results by provided Func
    /// </summary>
    public class FuncDimension : IDimension
    {
        private readonly Func<TestContextResult, string> _dimensionValueSelector;

        /// <param name="dimensionValueSelector">Dimension value selector</param>
        public FuncDimension(Func<TestContextResult,string> dimensionValueSelector)
        {
            if (dimensionValueSelector == null)
                throw new ArgumentNullException(nameof(dimensionValueSelector));

            _dimensionValueSelector = dimensionValueSelector;
        }

        string IDimension.GetKey(TestContextResult result)
        {
            return _dimensionValueSelector(result);
        }

        void IDimension.SetBegin(DateTime testBeginTime)
        {
        }
    }
}