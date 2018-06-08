using System;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario
{
    public static class CheckpointExtensions
    {
        public static TimeSpan Diff(this ICheckpoint checkpoint, ICheckpoint next)
        {
            return next.TimePoint - checkpoint.TimePoint;
        }
    }
}