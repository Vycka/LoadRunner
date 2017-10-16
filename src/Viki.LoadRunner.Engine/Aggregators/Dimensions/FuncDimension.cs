using System;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Strategy.Stats.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Dimensions
{
    /// <summary>
    /// Split results by provided Func
    /// </summary>
    public class FuncDimension : IDimension
    {
        private readonly Func<IResult, string> _dimensionValueSelector;

        /// <param name="dimensionName">Name/Key of custom dimension</param>
        /// <param name="dimensionValueSelector">Dimension value selector</param>
        public FuncDimension(string dimensionName, Func<IResult,string> dimensionValueSelector)
        {
            if (dimensionName == null)
                throw new ArgumentNullException(nameof(dimensionName));
            if (dimensionValueSelector == null)
                throw new ArgumentNullException(nameof(dimensionValueSelector));

            _dimensionValueSelector = dimensionValueSelector;
            DimensionName = dimensionName;
        }

        public string DimensionName { get; }

        string IDimension.GetKey(IResult result)
        {
            return _dimensionValueSelector(result);
        }
    }
}