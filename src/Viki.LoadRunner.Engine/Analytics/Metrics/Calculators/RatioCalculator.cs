namespace Viki.LoadRunner.Engine.Analytics.Metrics.Calculators
{
    public class RatioCalculator
    {
        public void Add(bool increaseRatio)
        {
            TotalCount++;

            if (increaseRatio)
                RatioCount++;
        }

        public int TotalCount { get; private set; } = 0;
        public int RatioCount { get; private set; } = 0;

        public double Ratio => (double)RatioCount / TotalCount;
    }
}