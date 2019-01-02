using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    /// <summary>
    /// Calculates percentage value of failed iterations (those which throw exception) 
    /// </summary>
    public class ErrorRatioMetric : IMetric
    {
        private readonly FlexiRow<string, ErrorRatio> _counts;

        private readonly HashSet<string> _ignoredCheckpoints;

        /// <summary>
        /// Calculates percentage value of failed iterations (those which throw exception) 
        /// </summary>
        /// <param name="ignoredCheckpoints">Checkpoints to ignore</param>
        public ErrorRatioMetric(params string[] ignoredCheckpoints)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = new HashSet<string>(ignoredCheckpoints);

            _counts = new FlexiRow<string, ErrorRatio>(() => new ErrorRatio());
        }

        IMetric<IResult> IMetric<IResult>.CreateNew()
        {
            return new ErrorRatioMetric(_ignoredCheckpoints.ToArray());
        }

        void IMetric<IResult>.Add(IResult result)
        {

            ICheckpoint[] checkpoints = result.Checkpoints;
            for (int i = 0, j = checkpoints.Length; i < j; i++)
            {
                ICheckpoint checkpoint = checkpoints[i];
                if (checkpoint.Error != null && _ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    if (_ignoredCheckpoints.Contains(checkpoint.Name))
                        continue;

                    _counts[checkpoint.Name].Increase(checkpoint.Error != null);
                }
            }
        }

        string[] IMetric<IResult>.ColumnNames => _counts
            .Where(kv => kv.Value.HasErrors)
            .Select(kv => String.Concat("ErrRatio: ", kv.Key))
            .ToArray();

        object[] IMetric<IResult>.Values => _counts
            .Where(kv => kv.Value.HasErrors)
            .Select(kv => kv.Value.Ratio)
            .Cast<object>()
            .ToArray();

        private class ErrorRatio
        {
            public void Increase(bool isError)
            {
                _requestCount++;

                if (isError)
                    _errorCount++;
            }

            private int _requestCount = 0;
            private int _errorCount = 0;

            public double Ratio => (100.0 / _requestCount) * _errorCount;

            public bool HasErrors => _errorCount > 0;
        }
    }
}