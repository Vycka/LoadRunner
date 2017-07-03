using System;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Threads;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    /// <summary>
    /// Define manually a table of various speed values which will be used one by one every provided time period
    /// </summary>
    public class ListOfSpeed : FixedSpeed, ISpeedStrategy
    {
        private readonly TimeSpan _period;
        private readonly double[] _iterationPerSecValues;

        protected static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Define manually a table of various speed values which will be used one by one every provided time period
        /// </summary>
        /// <param name="period">time period after which value from the next index will be used</param>
        /// <param name="iterationPerSecValues">Speed values for each time period.
        /// Once all values are consumed, speed will stay at value from last index</param>
        public ListOfSpeed(TimeSpan period, params double[] iterationPerSecValues) : base(Double.MaxValue)
        {
            if (iterationPerSecValues == null)
                throw new ArgumentNullException(nameof(iterationPerSecValues));
            if (iterationPerSecValues.Length == 0)
                throw new ArgumentException("At least one iteration speed must be provided", nameof(iterationPerSecValues));

            _period = period;
            _iterationPerSecValues = iterationPerSecValues;

            SetSpeed(iterationPerSecValues[0], TimeSpan.Zero);
        }

        protected int GetIndex(TimeSpan timerValue)
        {
            int index = (int)(timerValue.Ticks / _period.Ticks);

            if (index >= _iterationPerSecValues.Length)
                index = _iterationPerSecValues.Length - 1;

            return index;
        }

        private int _currentIndex = -1;

        public new void Adjust(IThreadPoolContext context)
        {
            int index = GetIndex(context.Timer.Value);
            if (index != _currentIndex)
            {
                _currentIndex = index;

                SetSpeed(_iterationPerSecValues[_currentIndex], context.Timer.Value);
            }
            

            base.Adjust(context);
        }
    }
}