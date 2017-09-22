using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Framework;

namespace Viki.LoadRunner.Engine.Strategies.Speed
{
    public class MaxSpeed : ISpeedStrategy
    {
        public void Next(ITestState state, ISchedule schedule)
        {
            schedule.Execute();
        }

        public void HeartBeat(ITestState state)
        {
        }
    }
}