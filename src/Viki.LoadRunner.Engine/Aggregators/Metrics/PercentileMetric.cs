using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Analytics.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class PercentileMetric : IMetric
    {
        private readonly FlexiRow<string, List<double>> _row = new FlexiRow<string, List<double>>(() => new List<double>());
        private readonly double[] _percentiles;
        private readonly string[] _ignoredCheckpoints;

        private readonly FlexiRow<string, long> _percentileCache = new FlexiRow<string, long>((() => default(long))); 
        private bool _percentileValueCacheValid;

        /// <summary>
        /// Custom formatter, allows to post-process the value before outputing it to the grid.
        /// </summary>
        public Func<long, object> Formatter = (duration) => duration;

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
            _ignoredCheckpoints = ignoredCheckpoints.Union(Checkpoint.NotMeassuredCheckpoints).ToArray();
        }

        IMetric<IResult> IMetric<IResult>.CreateNew()
        {
            return new PercentileMetric(_percentiles, _ignoredCheckpoints)
            {
                Formatter = Formatter
            };
        }

        void IMetric<IResult>.Add(IResult result)
        {
            _percentileValueCacheValid = false;

            ICheckpoint[] checkpoints = result.Checkpoints;
            for (int i = 0, j = checkpoints.Length - 1; i < j; i++)
            {
                ICheckpoint checkpoint = checkpoints[i];
                if (checkpoint.Error == null && _ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    TimeSpan momentDiff = checkpoint.Diff(checkpoints[i + 1]);

                    _row[checkpoint.Name].Add(momentDiff.TotalMilliseconds);
                }
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
                        string key = string.Concat(targetPercentile*100, "%: ", pair.Key);

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