using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    public class DimensionsKeyBuilder
    {
        private readonly IDimension[] _dimensions;

        public DimensionsKeyBuilder(IEnumerable<IDimension> dimensions)
        {
            _dimensions = dimensions.ToArray();
        }

        public DimensionKey GetValue(IResult result)
        {
            string[] dimensionValues = _dimensions.Select(d => d.GetKey(result)).ToArray();

            DimensionKey resultKey = new DimensionKey(dimensionValues);

            return resultKey;
        }
    }
}