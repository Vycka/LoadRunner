using System.Collections.Generic;
using System.Linq;

namespace Viki.LoadRunner.Tools.Analytics
{
    public class DimensionsGroup<T>
    {
        private readonly IDimension<T>[] _dimensions;

        public DimensionsGroup(IEnumerable<IDimension<T>> dimensions)
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
