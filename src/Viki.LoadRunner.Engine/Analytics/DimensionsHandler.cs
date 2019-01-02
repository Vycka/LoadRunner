using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics
{
    public class DimensionsHandler<T>
    {
        private readonly IDimension<T>[] _dimensions;

        public DimensionsHandler(IEnumerable<IDimension<T>> dimensions)
        {
            _dimensions = dimensions.ToArray();
        }

        public DimensionKey GetValue(T result)
        {
            string[] dimensionValues = _dimensions.Select(d => d.GetKey(result)).ToArray();

            DimensionKey resultKey = new DimensionKey(dimensionValues);

            return resultKey;
        }
    }
}
