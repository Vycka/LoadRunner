using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Utils
{
    public class DimensionsKeyBuilder
    {
        private readonly IDimension[] _dimensions;

        public DimensionsKeyBuilder(IEnumerable<IDimension> dimensions)
        {
            _dimensions = dimensions.ToArray();
        }

        public DimensionValues GetValue(IResult result)
        {
            string[] dimensionValues = _dimensions.Select(d => d.GetKey(result)).ToArray();

            DimensionValues resultValue = new DimensionValues(dimensionValues);

            return resultValue;
        }
    }
}