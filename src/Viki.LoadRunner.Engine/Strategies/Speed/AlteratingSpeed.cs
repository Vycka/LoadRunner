using System;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    //public class AlteratingSpeed : ISpeedStrategy
    //{
    //    private readonly TimeSpan _firstDuration;
    //    private readonly TimeSpan _firstDelay;
    //    private readonly TimeSpan _secondDelay;
    //    private TimeSpan _timeFrameDuration;

    //    public AlteratingSpeed(TimeSpan firstDuration, double firstRequetsPerSec, TimeSpan secondDuration, double secondRequestsPerSec)
    //    {
    //        long firstDelayTicks = (long)(TimeSpan.TicksPerSecond / firstRequetsPerSec);
    //        long secondDelayTicks = (long)(TimeSpan.TicksPerSecond / secondRequestsPerSec);

    //        _firstDelay = TimeSpan.FromTicks(firstDelayTicks);
    //        _secondDelay = TimeSpan.FromTicks(secondDelayTicks);

    //        _firstDuration = firstDuration;

    //        _timeFrameDuration = firstDuration + secondDuration;
    //    }


    //    TimeSpan ISpeedStrategy.GetDelayBetweenIterations(TimeSpan testExecutionTime)
    //    {
    //        TimeSpan timeFramePosition = TimeSpan.FromTicks(testExecutionTime.Ticks % _timeFrameDuration.Ticks);

    //        if (timeFramePosition <= _firstDuration)
    //        {
    //            TimeSpan

    //            if (_firstDelay >= _firstDuration)
    //                return
    //        }
    //            return _firstDelay;

    //        return _secondDelay;

    //    }
    //}
}