using System;
using System.Linq;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    /// <summary>
    /// Define manually a table of various speed values which will be used one by one every provided time period
    /// </summary>
    public class ListOfSpeed : ISpeedStrategyLegacy
    {
        private readonly TimeSpan _period;
        private readonly double[] _iterationPerSecValues;

        private readonly TimeSpan _maxSpeed;
        protected static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Define manually a table of various speed values which will be used one by one every provided time period
        /// </summary>
        /// <param name="period">time period after which value from the next index will be used</param>
        /// <param name="iterationPerSecValues">Speed values for each time period.
        /// Once all values are consumed, speed will stay at value from last index</param>
        public ListOfSpeed(TimeSpan period, params double[] iterationPerSecValues)
        {
            if (iterationPerSecValues == null)
                throw new ArgumentNullException(nameof(iterationPerSecValues));
            if (iterationPerSecValues.Length == 0)
                throw new ArgumentException("At least one iteration speed must be provided", nameof(iterationPerSecValues));

            _period = period;
            _iterationPerSecValues = iterationPerSecValues;
            _maxSpeed = TimeSpan.FromTicks((long)(OneSecond.Ticks /  _iterationPerSecValues.Last()));
        }

        public virtual TimeSpan GetDelayBetweenIterations(TimeSpan testExecutionTime)
        {
            long index = testExecutionTime.Ticks / _period.Ticks;

            if (index < _iterationPerSecValues.Length)
                return TimeSpan.FromTicks((long) (OneSecond.Ticks / _iterationPerSecValues[index])); // TODO: This math can be cached in c-tor

            return _maxSpeed;
        }
    }
}