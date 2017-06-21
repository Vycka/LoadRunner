using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class PercentileMetric : IMetric
    {
        private readonly FlexiRow<string, List<double>> _row = new FlexiRow<string, List<double>>(() => new List<double>());
        private readonly double[] _percentiles;
        private readonly string[] _ignoredCheckpoints;

        private static readonly Checkpoint BlankCheckpoint = new Checkpoint("", TimeSpan.Zero);

        private readonly FlexiRow<string, long> _percentileCache = new FlexiRow<string, long>((() => default(long))); 
        private bool _percentileValueCacheValid;

        /// <summary>
        /// Custom formatter, allows to post-process the value before outputing it to the grid.
        /// </summary>
        public Func<long, object> Formatter => (duration) => duration;

        public PercentileMetric(params double[] percentiles)
            : this(percentiles, new string[] { })
        {
        }

        public PercentileMetric(double[] percentiles, string[] ignoredCheckpoints)
        {
            if (percentiles == null)
                throw new ArgumentNullException(nameof(percentiles));
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _percentiles = percentiles;
            _ignoredCheckpoints = ignoredCheckpoints;
        }

        IMetric IMetric.CreateNew()
        {
            return new PercentileMetric(_percentiles, _ignoredCheckpoints);
        }

        void IMetric.Add(IResult result)
        {
            _percentileValueCacheValid = false;

            ICheckpoint previousCheckpoint = BlankCheckpoint;
            foreach (ICheckpoint checkpoint in result.Checkpoints)
            {
                if (_ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    TimeSpan momentDiff = TimeSpan.FromTicks(checkpoint.TimePoint.Ticks - previousCheckpoint.TimePoint.Ticks);

                    _row[checkpoint.Name].Add(momentDiff.TotalMilliseconds); 
                }
                previousCheckpoint = checkpoint;
            }
        }

        public string[] ColumnNames => GetPercentileValues().Keys.ToArray();
        public object[] Values => GetPercentileValues().Values.Select(value => Formatter(value)).ToArray();

        private FlexiRow<string, long> GetPercentileValues()
        {
            if (!_percentileValueCacheValid)
            {
                foreach (KeyValuePair<string, List<double>> pair in _row)
                {
                    pair.Value.Sort();
                    double[] sortedData = pair.Value.ToArray();

                    foreach (double targetPercentile in _percentiles)
                    {
                        string key = string.Concat(Math.Round(targetPercentile*100, 1), "%: ", pair.Key);

                        _percentileCache[key] = Convert.ToInt64(CalculatePercentile(sortedData, targetPercentile));
                    }
                }       

                _percentileValueCacheValid = true;
            }

            return _percentileCache;
        }

        private static double CalculatePercentile(double[] sortedData, double percentile)
        {
            double realIndex = percentile * (sortedData.Length - 1);
            int index = (int)realIndex;
            double frac = realIndex - index;
            if (index + 1 < sortedData.Length)
                return sortedData[index] * (1 - frac) + sortedData[index + 1] * frac;
            else
                return sortedData[index];
        }
    }
}