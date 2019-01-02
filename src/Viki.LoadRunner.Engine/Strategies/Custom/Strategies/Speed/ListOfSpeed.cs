using System;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
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
                throw new ArgumentException("At least one iteration speed must be provided",
                    nameof(iterationPerSecValues));

            _period = period;
            _iterationPerSecValues = iterationPerSecValues;

            HeartBeatInner(TimeSpan.Zero);
        }

        protected int GetIndex(TimeSpan timerValue)
        {
            int index = (int)(timerValue.Ticks / _period.Ticks);

            if (index >= _iterationPerSecValues.Length)
                index = _iterationPerSecValues.Length - 1;

            return index;
        }

        
        public new void HeartBeat(ITestState state)
        {
            HeartBeatInner(state.Timer.Value);
            base.HeartBeat(state);
        }

        private int _currentIndex = -1;

        private void HeartBeatInner(TimeSpan timerValue)
        {
            int index = GetIndex(timerValue);
            if (index != _currentIndex)
            {
                _currentIndex = index;

                SetSpeed(_iterationPerSecValues[_currentIndex], timerValue);
            } 
        }
    }
}