using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Collector.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;

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

        IMetric IMetric.CreateNew()
        {
            return new ErrorRatioMetric(_ignoredCheckpoints.ToArray());
        }

        void IMetric.Add(IResult result)
        {
            foreach (ICheckpoint checkpoint in result.Checkpoints)
            {
                if (_ignoredCheckpoints.Contains(checkpoint.Name))
                    continue;

                _counts[checkpoint.Name].Increase(checkpoint.Error != null);
            }
        }

        string[] IMetric.ColumnNames => _counts
            .Where(kv => kv.Value.HasErrors)
            .Select(kv => String.Concat("ErrRatio: ", kv.Key))
            .ToArray();

        object[] IMetric.Values => _counts
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