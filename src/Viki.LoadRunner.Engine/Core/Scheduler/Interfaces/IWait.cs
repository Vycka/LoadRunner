using System;

namespace Viki.LoadRunner.Engine.Core.Scheduler.Interfaces
{
    public interface IWait
    {
        void Wait(TimeSpan target, ref bool cencellationToken);
    }
}