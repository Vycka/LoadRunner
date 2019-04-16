namespace Viki.LoadRunner.Engine.Analytics.Metrics.Calculators
{
    public class RatioCalculator
    {
        public void Add(bool increaseRatio)
        {
            _totalCount++;

            if (increaseRatio)
                _ratioCount++;
        }

        private int _totalCount = 0;
        private int _ratioCount = 0;

        public double Ratio => (double)_ratioCount / _totalCount;
    }
}