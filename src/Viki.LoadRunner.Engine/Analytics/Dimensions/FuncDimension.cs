using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Dimensions
{
    public class FuncDimension<T> : IDimension<T>
    {
        private readonly Func<T, string> _dimensionValueSelector;

        /// <param name="dimensionName">Name/Key of custom dimension</param>
        /// <param name="dimensionValueSelector">Dimension value selector</param>
        public FuncDimension(string dimensionName, Func<T, string> dimensionValueSelector)
        {
            if (dimensionName == null)
                throw new ArgumentNullException(nameof(dimensionName));
            if (dimensionValueSelector == null)
                throw new ArgumentNullException(nameof(dimensionValueSelector));

            _dimensionValueSelector = dimensionValueSelector;
            DimensionName = dimensionName;
        }

        public string DimensionName { get; }

        string IDimension<T>.GetKey(T result)
        {
            return _dimensionValueSelector(result);
        }
    }
}