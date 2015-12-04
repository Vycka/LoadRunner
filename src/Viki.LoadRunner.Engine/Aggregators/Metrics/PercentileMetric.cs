using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class PercentileMetric : IMetric
    {
        private readonly FlexiGrid<string, List<double>> _grid = new FlexiGrid<string, List<double>>((() => new List<double>()));
        private readonly double[] _percentiles;
        private readonly string[] _ignoredCheckpoints;

        private static readonly Checkpoint BlankCheckpoint = new Checkpoint("", TimeSpan.Zero);

        private readonly FlexiGrid<string, long> _percentileCache = new FlexiGrid<string, long>((() => default(long))); 
        private bool _percentileValueCacheValid;

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

        public IMetric CreateNew()
        {
            return new PercentileMetric(_percentiles, _ignoredCheckpoints);
        }

        public void Add(TestContextResult result)
        {
            _percentileValueCacheValid = false;

            Checkpoint previousCheckpoint = BlankCheckpoint;
            foreach (Checkpoint checkpoint in result.Checkpoints)
            {
                if (_ignoredCheckpoints.All(name => name != checkpoint.CheckpointName))
                {
                    TimeSpan momentDiff = TimeSpan.FromTicks(checkpoint.TimePoint.Ticks - previousCheckpoint.TimePoint.Ticks);

                    _grid[checkpoint.CheckpointName].Add(momentDiff.TotalMilliseconds); 
                }
                previousCheckpoint = checkpoint;
            }
        }

        public string[] ColumnNames => GetPercentileValues().Keys.ToArray();
        public object[] Values => GetPercentileValues().Values.Select(value => (object)value).ToArray();

        private FlexiGrid<string, long> GetPercentileValues()
        {
            if (!_percentileValueCacheValid)
            {
                foreach (KeyValuePair<string, List<double>> pair in _grid)
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