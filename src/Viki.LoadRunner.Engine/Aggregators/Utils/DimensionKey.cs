#pragma warning disable 1591

using System;
using System.Collections.Generic;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    public class DimensionKey

    {
        private readonly string[] _dimensionValues;
        private readonly string _cachedKey;

        public IReadOnlyList<string> Values => _dimensionValues;

        public DimensionKey(string[] dimensionValues)
        {
            if (dimensionValues == null)
                throw new ArgumentNullException(nameof(dimensionValues));

            _dimensionValues = dimensionValues;
            _cachedKey = String.Join(" ", dimensionValues);
        }


        public override bool Equals(object obj)
        {
            return (obj as DimensionKey)?._cachedKey == _cachedKey;
        }

        public override int GetHashCode()
        {
            return _cachedKey.GetHashCode();
        }
    }
}